namespace HomeManager.Core.Weather

open System

type WeatherEntry<[<Measure>] 'tempUnit> = {
    TimeOfDay: TimeOnly
    Data: WeatherData<'tempUnit>
} with

    [<CompiledName("Create")>]
    static member create timeOfDay data = { TimeOfDay = timeOfDay; Data = data }
