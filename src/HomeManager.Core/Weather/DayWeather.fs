namespace HomeManager.Core.Weather

open System
open System.Collections.Immutable

type DayWeather<[<Measure>] 'tempUnit> = {
    Day: DateOnly
    Entries: ImmutableArray<WeatherEntry<'tempUnit>>
} with

    member this.CalculateAverageTemperature<'tempUnit>() =
        this |> DayWeather.calculateAverageTemperature

    member this.CalculateAveragePrecipitation<'tempUnit>() =
        this |> DayWeather.calculateAveragePrecipitation

    member this.CalculateAverageHumidity<'tempUnit>() =
        this |> DayWeather.calculateAverageHumidity

    member this.CalculateAverageWindSpeed<'tempUnit>() =
        this |> DayWeather.calculateAverageWindSpeed

    [<CompiledName("Create")>]
    static member create day (entries: WeatherEntry<'tempUnit> seq) = {
        Day = day
        Entries = entries.ToImmutableArray()
    }

    [<CompiledName("CalculateAverageTemperature")>]
    static member calculateAverageTemperature<'tempUnit>(day: DayWeather<'tempUnit>) =
        if day.Entries.IsEmpty then
            raise (ArgumentException($"No weather entries exist for day {day}", nameof day.Entries))

        day.Entries |> Seq.averageBy (fun entry -> entry.Data.Temperature)

    [<CompiledName("CalculateAveragePrecipitation")>]
    static member calculateAveragePrecipitation<'tempUnit>(day: DayWeather<'tempUnit>) =
        if day.Entries.IsEmpty then
            raise (ArgumentException($"No weather entries exist for day {day}", nameof day.Entries))

        day.Entries |> Seq.averageBy (fun entry -> entry.Data.Precipitation)

    [<CompiledName("CalculateAverageHumidity")>]
    static member calculateAverageHumidity<'tempUnit>(day: DayWeather<'tempUnit>) =
        if day.Entries.IsEmpty then
            raise (ArgumentException($"No weather entries exist for day {day}", nameof day.Entries))

        day.Entries |> Seq.averageBy (fun entry -> entry.Data.Humidity)

    [<CompiledName("CalculateAverageWindSpeed")>]
    static member calculateAverageWindSpeed<'tempUnit>(day: DayWeather<'tempUnit>) =
        if day.Entries.IsEmpty then
            raise (ArgumentException($"No weather entries exist for day {day}", nameof day.Entries))

        day.Entries |> Seq.averageBy (fun entry -> entry.Data.WindSpeed)
