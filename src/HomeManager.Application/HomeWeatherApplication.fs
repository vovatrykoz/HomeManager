namespace HomeManager.Application

open HomeManager.Core
open HomeManager.Core.Weather
open System

type NoForecastException(msg: string) =
    inherit Exception(msg)

type NoTodayWeatherDataException(msg: string, today: DateOnly) =
    inherit Exception(msg)

    member _.Today = today

type HomeWeatherApplication<[<Measure>] 'tempUnit>
    (
        weatherProvider: IWeatherProvider<'tempUnit>,
        weatherDisplay: IWeatherDisplay<'tempUnit>,
        timeService: ITimeService,
        logger: ILogger,
        ?forecastRange: int
    ) =
    member _.WeatherProvider = weatherProvider

    member _.WeatherDisplay = weatherDisplay

    member _.TimeService = timeService

    member _.Logger = logger

    member val ForecastRangeDays = defaultArg forecastRange 7 with get, set

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
                let forecastEndDay = today.AddDays this.ForecastRangeDays
                let! forecast = weatherProvider.GetWeatherAsync today forecastEndDay

                let filteredForecast =
                    forecast |> Array.sortBy (fun e -> e.Day) |> Array.distinctBy (fun e -> e.Day)

                if Array.isEmpty filteredForecast then
                    this.Logger.LogError "No weather data returned by the provider"
                    raise (NoForecastException $"No weather data returned by the provider")

                let weatherToday = Array.head filteredForecast
                let currentTime = TimeOnly.FromDateTime now

                if weatherToday.Entries.IsEmpty then
                    this.Logger.LogError $"No weather data available for today ({weatherToday.Day})"

                    raise (
                        NoTodayWeatherDataException(
                            $"No weather data available for today ({weatherToday.Day})",
                            weatherToday.Day
                        )
                    )

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
