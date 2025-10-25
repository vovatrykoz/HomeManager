namespace HomeManager.Application

open HomeManager.Core.Weather
open System
open System.Threading.Tasks

[<Interface>]
type IWeatherProvider<[<Measure>] 'tempUnit> =

    abstract member UpdateNeeded: bool

    abstract member GetWeatherAsync<'tempUnit> :
        startDate: DateOnly -> endDate: DateOnly -> Task<DayWeather<'tempUnit> array>
