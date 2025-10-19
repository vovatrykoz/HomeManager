namespace HomeManager.Core.Weather

open System
open System.Collections.Immutable

type DayWeather<[<Measure>] 'tempUnit> = {
    Day: DateOnly
    Entries: ImmutableArray<WeatherEntry<'tempUnit>>
} with

    member private this.LazyTempAverage =
        lazy (this.Entries |> Seq.averageBy (fun entry -> entry.Data.Temperature))

    member this.AverageTemperature = this.LazyTempAverage.Force()

    member private this.LazyPrecipAverage =
        lazy (this.Entries |> Seq.averageBy (fun entry -> entry.Data.Precipitation))

    member this.AveragePrecipitation = this.LazyPrecipAverage.Force()

    member private this.LazyHumidAverage =
        lazy (this.Entries |> Seq.averageBy (fun entry -> entry.Data.Humidity))

    member this.AverageHumidity = this.LazyHumidAverage.Force()

    member private this.LazyWindAverage =
        lazy (this.Entries |> Seq.averageBy (fun entry -> entry.Data.Wind))

    member this.AverageWindSpeed = this.LazyWindAverage.Force()

    [<CompiledName("Create")>]
    static member create day (entries: WeatherEntry<'tempUnit> seq) = {
        Day = day
        Entries = entries.ToImmutableArray()
    }
