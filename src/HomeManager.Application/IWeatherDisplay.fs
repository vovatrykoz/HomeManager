namespace HomeManager.Application

open HomeManager.Core.Weather
open System.Threading.Tasks

[<Interface>]
type IWeatherDisplay<[<Measure>] 'tempUnit> =
    abstract member DisplayEntry: weatherEntry: WeatherEntry<'tempUnit> -> unit

    abstract member DisplayTimeline: weatherEntries: WeatherEntry<'tempUnit> seq -> unit

    abstract member DisplayForecast: forecast: DayWeather<'tempUnit> seq -> unit
