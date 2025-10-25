namespace HomeManager.Avalonia

open HomeManager.Application
open HomeManager.Core.Weather
open Avalonia.FuncUI

type AvaloniaWeatherDisplay<[<Measure>] 'tempUnit>(weatherPresenter: IWritable<SimpleWeatherPresenter<'tempUnit>>) =

    let _lock = obj ()

    interface IWeatherDisplay<'tempUnit> with
        member _.DisplayEntry<'tempUnit> entry =
            lock _lock (fun _ ->
                weatherPresenter.Current.MaxTemperature <- entry.Data.GetTemperature
                weatherPresenter.Set weatherPresenter.Current)

        member _.DisplayTimeline<'tempUnit> entries =
            raise (System.NotImplementedException())

        member _.DisplayForecast<'tempUnit> days =
            raise (System.NotImplementedException())
