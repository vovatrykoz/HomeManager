namespace HomeManager.Core.Weather

open System

type WeatherEntry = {
    TimeOfDay: TimeOnly
    Data: WeatherData
} with

    [<CompiledName("Create")>]
    static member create timeOfDay data = { TimeOfDay = timeOfDay; Data = data }
