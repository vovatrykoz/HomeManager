namespace HomeManager.Avalonia

open HomeManager.Core.Weather
open System.Collections.Generic

type SimpleForecastPresenter<[<Measure>] 'tempUnit>(forecast: DayWeather<'tempUnit> seq) =

    member _.Forecast = List<DayWeather<'tempUnit>> forecast
