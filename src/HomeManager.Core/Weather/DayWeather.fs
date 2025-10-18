namespace HomeManager.Core.Weather

open System

type DayWeather = {
    Day: DateOnly
    Entries: WeatherEntry array
} with

    static member create day entries = { Day = day; Entries = entries }
