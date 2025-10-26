namespace HomeManager.Avalonia

open HomeManager.Core.Weather
open System.Collections.Generic

type SimpleTimelinePresenter<[<Measure>] 'tempUnit>(timeline: WeatherEntry<'tempUnit> seq) =

    member _.Timeline = List<WeatherEntry<'tempUnit>> timeline
