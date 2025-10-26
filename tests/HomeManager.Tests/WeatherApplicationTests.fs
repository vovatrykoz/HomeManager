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

    member _.AccessCounter
        with get () = _counter
        and private set value = _counter <- value

    interface ITimeService with
        member this.GetCurrentTime() : DateTime =
            this.AccessCounter <- this.AccessCounter + 1
            this.CurrentTime

type MockWeatherProvider(updateNeeded: bool, rawData: (DateTime * (DateTime * WeatherData<celsius>) list) list) =
    let mutable _counter = 0

    member _.GetWeatherCallCounter = _counter

    interface IWeatherProvider<celsius> with
        member _.UpdateNeeded = updateNeeded

        member _.GetWeatherAsync (startDate: DateOnly) (endDate: DateOnly) : Task<DayWeather<celsius> array> =
            _counter <- _counter + 1

            task {
                return
                    rawData
                    |> List.map (fun (dayTime, rawEntries) ->
                        let entries =
                            rawEntries
                            |> List.map (fun (time, rawEntry) ->
                                WeatherEntry<_>.create (TimeOnly.FromDateTime time) rawEntry)
                            |> List.toSeq

                        let day = DateOnly.FromDateTime dayTime

                        DayWeather<_>.create day entries)
                    |> List.filter (fun dayData ->
                        if dayData.Day >= startDate && dayData.Day <= endDate then
                            true
                        else
                            false)
                    |> List.toArray
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

module ``Home Weather Application Tests`` =
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

    [<Property>]
    let ``HomeWeatherApplication returns early if no update is needed``
        (rawData: (DateTime * (DateTime * WeatherData<celsius>) list) list)
        =
        let weatherProvider = MockWeatherProvider(updateNeeded = false, rawData = rawData)
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

    [<Property>]
    let ``HomeWeatherApplication does not return early if update is needed``
        (rawData: (DateTime * (DateTime * WeatherData<celsius>) list) list)
        =
        let weatherProvider = MockWeatherProvider(updateNeeded = true, rawData = rawData)
        let weatherDisplay = MockWeatherDisplay()
        let timeService = MockTimeService()
        let logger = NoOpLogger()

        let startTime = (timeService :> ITimeService).GetCurrentTime()
        let endTime = startTime.AddDays(HomeWeatherApplication<_>.ForecastRange)

        let app =
            HomeWeatherApplication(weatherProvider, weatherDisplay, timeService, logger)

        let expected =
            rawData
            |> List.map (fun (dayTime, rawEntries) ->
                let entries =
                    rawEntries
                    |> List.map (fun (time, rawEntry) -> WeatherEntry<_>.create (TimeOnly.FromDateTime time) rawEntry)
                    |> List.toSeq

                let day = DateOnly.FromDateTime dayTime

                DayWeather<_>.create day entries)
            |> List.collect (fun data ->
                data.Entries
                |> Seq.toList
                |> List.choose (fun entry ->
                    let exactTime = DateTime(data.Day, entry.TimeOfDay)

                    if exactTime >= startTime && exactTime <= endTime then
                        Some entry
                    else
                        None))

        if List.isEmpty expected then
            Assert.Throws<AggregateException>(fun () -> app.RunAsync().Wait()) |> ignore
        else
            app.RunAsync() |> Async.AwaitTask |> Async.RunSynchronously

            Assert.Multiple(fun () ->
                Assert.That(timeService.AccessCounter, Is.EqualTo 1)
                Assert.That(weatherProvider.GetWeatherCallCounter, Is.EqualTo 1)
                Assert.That(weatherDisplay.EntryDisplayCounter, Is.EqualTo 1)
                Assert.That(weatherDisplay.ForecastDisplayCounter, Is.EqualTo 1)
                Assert.That(weatherDisplay.TimelineDisplayCounter, Is.EqualTo 1))
