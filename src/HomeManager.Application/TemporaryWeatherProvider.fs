namespace HomeManager.Application

open HomeManager.Core.Weather
open System
open System.Threading.Tasks

type TemporaryWeatherProvider() =
    let mutable _temperature = 1.0f<celsius>

    let mutable _lastUpdateTime = DateTime.Now

    member _.LastUpdateTime = _lastUpdateTime

    member _.UpdateInterval = TimeSpan.FromSeconds 3

    interface IWeatherProvider<celsius> with
        member this.UpdateNeeded =
            let now = DateTime.Now
            let interval = now - this.LastUpdateTime

            if interval > this.UpdateInterval then
                _temperature <- _temperature + 1.0f<celsius>
                _lastUpdateTime <- now
                true
            else
                false

        member _.GetWeatherAsync (startDate: DateOnly) (endDate: DateOnly) : Task<DayWeather<celsius> array> =
            task {
                let data =
                    WeatherData<_>.create _temperature 0.0f<percent> 0.0f<percent> 0.0f<meters / second>

                let entries =
                    [
                        (WeatherEntry<_>.create (TimeOnly.FromDateTime(startDate.ToDateTime(TimeOnly(5, 0)))) data)
                    ]
                    |> List.toSeq

                return [| (DayWeather.create startDate entries) |]
            }
