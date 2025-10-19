namespace HomeManager.Core.Weather

open System
open System.Collections.Immutable

type DayWeather<[<Measure>] 'tempUnit> = {
    Day: DateOnly
    Entries: ImmutableArray<WeatherEntry<'tempUnit>>
} with

    member private this.LazyTempAverage =
        lazy (this |> DayWeather.calculateAverageTemperature)

    member this.AverageTemperature = this.LazyTempAverage.Force()

    member private this.LazyPrecipAverage =
        lazy (this |> DayWeather<_>.calculateAveragePrecipitation)

    member this.AveragePrecipitation = this.LazyPrecipAverage.Force()

    member private this.LazyHumidAverage =
        lazy (this |> DayWeather<_>.calculateAverageHumidity)

    member this.AverageHumidity = this.LazyHumidAverage.Force()

    member private this.LazyWindAverage =
        lazy (this |> DayWeather<_>.calculateAverageWindSpeed)

    member this.AverageWindSpeed = this.LazyWindAverage.Force()

    static member create day (entries: WeatherEntry<'tempUnit> seq) = {
        Day = day
        Entries = entries.ToImmutableArray()
    }

    static member calculateAverageTemperature<[<Measure>] 'tempUnit>
        (dayWeather: DayWeather<'tempUnit>)
        : float32<'tempUnit> =
        dayWeather.Entries |> Seq.averageBy (fun entry -> entry.Data.Temperature)

    static member calculateAveragePrecipitation dayWeather =
        dayWeather.Entries |> Seq.averageBy (fun entry -> entry.Data.Precipitation)

    static member calculateAverageHumidity dayWeather =
        dayWeather.Entries |> Seq.averageBy (fun entry -> entry.Data.Humidity)

    static member calculateAverageWindSpeed dayWeather =
        dayWeather.Entries |> Seq.averageBy (fun entry -> entry.Data.Wind)
