namespace HomeManager.Application

open HomeManager.Core.Weather

[<Interface>]
type IWeatherDisplay<[<Measure>] 'tempUnit> =

    abstract member DisplayEntry<'tempUnit> : weatherEntry: WeatherEntry<'tempUnit> -> unit

    abstract member DisplayTimeline<'tempUnit> : weatherEntries: WeatherEntry<'tempUnit> seq -> unit

    abstract member DisplayForecast<'tempUnit> : forecast: DayWeather<'tempUnit> seq -> unit
