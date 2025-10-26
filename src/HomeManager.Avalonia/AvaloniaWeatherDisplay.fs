namespace HomeManager.Avalonia

open Avalonia.FuncUI
open HomeManager.Application
open HomeManager.Core.Weather
open System.Collections.ObjectModel

type AvaloniaWeatherDisplay<[<Measure>] 'tempUnit>
    (
        weatherPresenter: IWritable<SimpleWeatherPresenter<'tempUnit>>,
        timelinePresenter: IWritable<ObservableCollection<WeatherEntry<'tempUnit>>>,
        forecastPresenter: IWritable<ObservableCollection<DayWeather<'tempUnit>>>
    ) =

    let _entryLock = obj ()

    let _timelineLock = obj ()

    let _forecastLock = obj ()

    interface IWeatherDisplay<'tempUnit> with
        member _.DisplayEntry<'tempUnit> entry =
            lock _entryLock (fun _ ->
                weatherPresenter.Current.MaxTemperature <- entry.Data.GetTemperature
                weatherPresenter.Current.Precipitation <- entry.Data.GetPrecipitation
                weatherPresenter.Current.Humidity <- entry.Data.GetHumidity
                weatherPresenter.Current.WindSpeed <- entry.Data.GetWindSpeed

                weatherPresenter.Set weatherPresenter.Current)

        member _.DisplayTimeline<'tempUnit> entries =
            lock _timelineLock (fun _ ->
                timelinePresenter.Current.Clear()
                timelinePresenter.Set(ObservableCollection entries))

        member _.DisplayForecast<'tempUnit> days =
            lock _forecastLock (fun _ ->
                forecastPresenter.Current.Clear()
                forecastPresenter.Set(ObservableCollection days))
