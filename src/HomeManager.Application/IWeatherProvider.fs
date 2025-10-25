namespace HomeManager.Application

open HomeManager.Core.Weather
open System
open System.Collections.Generic
open System.Threading.Tasks

[<Interface>]
type IWeatherProvider<[<Measure>] 'tempUnit> =
    abstract member GetWeatherAsync: startDate: DateOnly -> endDate: DateOnly -> Task<DayWeather<'tempUnit> array>
