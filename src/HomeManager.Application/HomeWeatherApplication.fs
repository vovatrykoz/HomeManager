namespace HomeManager.Application

open HomeManager.Core
open System

type HomeWeatherApplication<[<Measure>] 'tempUnit>
    (weatherProvider: IWeatherProvider<'tempUnit>, weatherDisplay: IWeatherDisplay<'tempUnit>, timeService: ITimeService)
    =
    member _.WeatherProvider = weatherProvider

    member _.WeatherDisplay = weatherDisplay

    member _.TimeService = timeService

    static member ForecastRange = 7

    member this.RunAsync() =
        task {
            let now = this.TimeService.GetCurrentTime()
            let today = now |> DateOnly.FromDateTime
            let nextWeek = today.AddDays HomeWeatherApplication<_>.ForecastRange

            let! forecast = weatherProvider.GetWeatherAsync today nextWeek
            let weatherToday = forecast.[0]
            let currentTime = TimeOnly.FromDateTime now

            let currentWeather =
                weatherToday.Entries
                |> Seq.minBy (fun entry -> abs (entry.TimeOfDay - currentTime).Ticks)

            this.WeatherDisplay.DisplayEntry currentWeather
        }
