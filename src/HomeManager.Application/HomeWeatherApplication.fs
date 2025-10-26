namespace HomeManager.Application

open HomeManager.Core
open HomeManager.Core.Weather
open System

type EmptyWeatherDataException(msg: string) =
    inherit Exception(msg)

type HomeWeatherApplication<[<Measure>] 'tempUnit>
    (
        weatherProvider: IWeatherProvider<'tempUnit>,
        weatherDisplay: IWeatherDisplay<'tempUnit>,
        timeService: ITimeService,
        logger: ILogger
    ) =
    member _.WeatherProvider = weatherProvider

    member _.WeatherDisplay = weatherDisplay

    member _.TimeService = timeService

    member _.Logger = logger

    static member ForecastRange = 7

    static member GetAllEntriesInRange(startTime: DateTime, endTime: DateTime, forecast: DayWeather<'tempUnit> array) =
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

    member this.RunAsync() =
        task {
            if not weatherProvider.UpdateNeeded then
                return ()
            else
                this.Logger.LogInfo "Updating weather data"

                let now = this.TimeService.GetCurrentTime()
                let today = now |> DateOnly.FromDateTime
                let nextWeek = today.AddDays HomeWeatherApplication<_>.ForecastRange
                let! forecast = weatherProvider.GetWeatherAsync today nextWeek

                if Array.isEmpty forecast then
                    this.Logger.LogError "No weather data returned by the provider"
                    raise (EmptyWeatherDataException $"No weather data returned by the provider")

                let weatherToday = Array.head forecast
                let currentTime = TimeOnly.FromDateTime now

                let currentWeather =
                    weatherToday.Entries
                    |> Seq.minBy (fun entry -> abs (entry.TimeOfDay - currentTime).Ticks)

                let dayTimeSpan = now.AddDays 1

                let timeline =
                    HomeWeatherApplication.GetAllEntriesInRange(now, dayTimeSpan, forecast)

                this.Logger.LogInfo "Displaying updated weather"

                this.WeatherDisplay.DisplayEntry currentWeather
                this.WeatherDisplay.DisplayTimeline timeline
                this.WeatherDisplay.DisplayForecast forecast
        }
