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

                let stableData =
                    WeatherData<_>.create 0.0f<celsius> 0.0f<percent> 0.0f<percent> 0.0f<meters / second>

                let entries =
                    [
                        WeatherEntry<_>.create (TimeOnly(3, 0)) data
                        WeatherEntry<_>.create (TimeOnly(4, 0)) data
                        WeatherEntry<_>.create (TimeOnly(5, 0)) data
                    ]
                    |> List.toSeq

                let nextDayEntries =
                    [
                        WeatherEntry<_>.create (TimeOnly.FromDateTime(endDate.ToDateTime(TimeOnly(3, 0)))) stableData
                        WeatherEntry<_>.create (TimeOnly.FromDateTime(endDate.ToDateTime(TimeOnly(4, 0)))) stableData
                        WeatherEntry<_>.create (TimeOnly.FromDateTime(startDate.ToDateTime(TimeOnly(5, 0)))) stableData
                        WeatherEntry<_>.create (TimeOnly.FromDateTime(startDate.ToDateTime(TimeOnly(6, 0)))) stableData
                        WeatherEntry<_>.create (TimeOnly.FromDateTime(startDate.ToDateTime(TimeOnly(7, 0)))) stableData
                        WeatherEntry<_>.create (TimeOnly.FromDateTime(startDate.ToDateTime(TimeOnly(8, 0)))) stableData
                    ]
                    |> List.toSeq

                return [|
                    DayWeather.create startDate entries
                    DayWeather.create (startDate.AddDays 1) nextDayEntries
                |]
            }
