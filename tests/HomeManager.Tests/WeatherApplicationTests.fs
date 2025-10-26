namespace HomeManager.Tests

open HomeManager.Core
open HomeManager.Core.Weather
open HomeManager.Application

open NUnit.Framework
open NUnit.Framework.Legacy
open FsCheck.NUnit
open System
open System.Threading.Tasks

type MockTimeService(currentTime: DateTime) =

    let mutable _counter = 0

    new() = MockTimeService DateTime.Now

    member val CurrentTime = currentTime with get, set

    member val AccessCounter = _counter with get

    interface ITimeService with
        member this.GetCurrentTime() : DateTime =
            _counter <- _counter + 1
            this.CurrentTime

type MockWeatherProvider(updateNeeded: bool) =
    let mutable _counter = 0

    member _.GetWeatherCallCounter = _counter

    interface IWeatherProvider<celsius> with
        member _.UpdateNeeded = updateNeeded

        member _.GetWeatherAsync (startDate: DateOnly) (endDate: DateOnly) : Task<DayWeather<celsius> array> =
            _counter <- _counter + 1

            task {

                let data =
                    WeatherData<_>.create 0.0f<celsius> 0.0f<percent> 0.0f<percent> 0.0f<meters / second>

                let entries =
                    [
                        (WeatherEntry<_>.create (TimeOnly.FromDateTime(startDate.ToDateTime(TimeOnly(5, 0)))) data)
                    ]
                    |> List.toSeq

                return [| (DayWeather.create startDate entries) |]
            }

type MockWeatherDisplay() =
    let mutable _entryCounter = 0

    let mutable _forecastCounter = 0

    let mutable _timelineCounter = 0

    member _.EntryDisplayCounter = _entryCounter

    member _.ForecastDisplayCounter = _forecastCounter

    member _.TimelineDisplayCounter = _timelineCounter

    interface IWeatherDisplay<celsius> with
        member _.DisplayEntry(_: WeatherEntry<celsius>) = _entryCounter <- _entryCounter + 1

        member _.DisplayForecast(_: DayWeather<celsius> seq) =
            _forecastCounter <- _forecastCounter + 1

        member _.DisplayTimeline(_: WeatherEntry<celsius> seq) =
            _timelineCounter <- _timelineCounter + 1

type NoOpLogger() =
    interface ILogger with
        member _.LogDebug(_: string) : unit = ()

        member _.LogError(_: string) : unit = ()

        member _.LogInfo(_: string) : unit = ()

        member _.LogWarning(_: string) : unit = ()

module ``Weather Application Tests`` =
    //
    [<Property>]
    let ``HomeWeatherApplication can correctly find entries in range``
        (startTime: DateTime)
        (endTime: DateTime)
        (rawData: (DateTime * (DateTime * WeatherData<celsius>) list) list)
        =
        let forecast =
            rawData
            |> List.map (fun (dayTime, rawEntries) ->
                let entries =
                    rawEntries
                    |> List.map (fun (time, rawEntry) -> WeatherEntry<_>.create (TimeOnly.FromDateTime time) rawEntry)
                    |> List.toSeq

                let day = DateOnly.FromDateTime dayTime

                DayWeather<_>.create day entries)
            |> List.toArray

        let expected =
            forecast
            |> Array.collect (fun data ->
                data.Entries
                |> Seq.toArray
                |> Array.choose (fun entry ->
                    let exactTime = DateTime(data.Day, entry.TimeOfDay)

                    if exactTime >= startTime && exactTime <= endTime then
                        Some entry
                    else
                        None))

        let actual =
            HomeWeatherApplication.GetAllEntriesInRange(startTime, endTime, forecast)

        CollectionAssert.AreEqual(expected, actual)

    [<Test>]
    let ``HomeWeatherApplication returns early if no update is needed`` () =
        let weatherProvider = MockWeatherProvider(updateNeeded = false)
        let weatherDisplay = MockWeatherDisplay()
        let timeService = MockTimeService()
        let logger = NoOpLogger()

        let app =
            HomeWeatherApplication(weatherProvider, weatherDisplay, timeService, logger)

        app.RunAsync() |> Async.AwaitTask |> Async.RunSynchronously

        Assert.Multiple(fun () ->
            Assert.That(timeService.AccessCounter, Is.Zero)
            Assert.That(weatherProvider.GetWeatherCallCounter, Is.Zero)
            Assert.That(weatherDisplay.EntryDisplayCounter, Is.Zero)
            Assert.That(weatherDisplay.ForecastDisplayCounter, Is.Zero)
            Assert.That(weatherDisplay.TimelineDisplayCounter, Is.Zero))
