namespace HomeManager.Avalonia

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open HomeManager.Core.Weather

module Main =
    open HomeManager.Application
    open System.Collections.ObjectModel

    let temperature =
        SimpleWeatherPresenter(0.0f<celsius>, 50.0f<percent>, 50.0f<percent>, 3.0f<meters / second>)

    let timeline = ObservableCollection Seq.empty
    let forecast = ObservableCollection Seq.empty

    let rec apiCallback (app: HomeWeatherApplication<celsius>) = app.RunAsync()

    let singleDayWeatherComponent (state: IWritable<SimpleWeatherPresenter<celsius>>) =
        Component(fun ctx ->
            let state = ctx.usePassed state

            DockPanel.create [
                DockPanel.children [
                    TextBlock.create [
                        TextBlock.dock Dock.Top
                        TextBlock.fontSize 48.0
                        TextBlock.verticalAlignment VerticalAlignment.Center
                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                        TextBlock.text $"Temperature: {state.Current.MaxTemperature} °C"
                    ]
                    TextBlock.create [
                        TextBlock.dock Dock.Top
                        TextBlock.fontSize 48.0
                        TextBlock.verticalAlignment VerticalAlignment.Center
                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                        TextBlock.text $"Precipitation: {state.Current.Precipitation} %%"
                    ]
                    TextBlock.create [
                        TextBlock.dock Dock.Top
                        TextBlock.fontSize 48.0
                        TextBlock.verticalAlignment VerticalAlignment.Center
                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                        TextBlock.text $"Humidity: {state.Current.Humidity} %%"
                    ]
                    TextBlock.create [
                        TextBlock.dock Dock.Top
                        TextBlock.fontSize 48.0
                        TextBlock.verticalAlignment VerticalAlignment.Center
                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                        TextBlock.text $"Precipitation: {state.Current.WindSpeed} m/s"
                    ]
                ]
            ])

    let singleTimelineComponent (state: IWritable<ObservableCollection<WeatherEntry<'tempUnit>>>) =
        Component(fun ctx ->
            let state = ctx.usePassedRead state

            StackPanel.create [
                StackPanel.children [
                    ItemsControl.create [
                        ItemsControl.dataItems state.Current
                        ItemsControl.itemTemplate (
                            DataTemplateView.create<_, _> (fun (entry: WeatherEntry<'tempUnit>) ->
                                DockPanel.create [
                                    DockPanel.margin 24
                                    DockPanel.children [
                                        TextBlock.create [
                                            TextBlock.dock Dock.Left
                                            TextBlock.fontSize 12.0
                                            TextBlock.verticalAlignment VerticalAlignment.Center
                                            TextBlock.horizontalAlignment HorizontalAlignment.Center
                                            TextBlock.text $"{entry.TimeOfDay} - {entry.Data.GetTemperature}"
                                        ]
                                    ]

                                ])
                        )
                    ]
                ]
            ])

    let forecastComponent (state: IWritable<ObservableCollection<DayWeather<'tempUnit>>>) =
        Component(fun ctx ->
            let state = ctx.usePassed state

            StackPanel.create [
                StackPanel.children [
                    ItemsControl.create [
                        ItemsControl.dataItems state.Current
                        ItemsControl.itemTemplate (
                            DataTemplateView.create<_, _> (fun (forecastEntry: DayWeather<'tempUnit>) ->
                                DockPanel.create [
                                    DockPanel.children [
                                        TextBlock.create [
                                            TextBlock.dock Dock.Top
                                            TextBlock.fontSize 12.0
                                            TextBlock.verticalAlignment VerticalAlignment.Center
                                            TextBlock.horizontalAlignment HorizontalAlignment.Center
                                            TextBlock.text $"{forecastEntry.Day}"
                                        ]
                                        StackPanel.create [
                                            StackPanel.children [
                                                ItemsControl.create [
                                                    ItemsControl.dataItems forecastEntry.Entries
                                                    ItemsControl.itemTemplate (
                                                        DataTemplateView.create<_, _>
                                                            (fun (entry: WeatherEntry<'tempUnit>) ->
                                                                DockPanel.create [
                                                                    DockPanel.children [
                                                                        TextBlock.create [
                                                                            TextBlock.dock Dock.Top
                                                                            TextBlock.fontSize 12.0
                                                                            TextBlock.verticalAlignment
                                                                                VerticalAlignment.Center
                                                                            TextBlock.horizontalAlignment
                                                                                HorizontalAlignment.Center
                                                                            TextBlock.text
                                                                                $"{entry.TimeOfDay} - {entry.Data.GetTemperature}"
                                                                        ]
                                                                    ]

                                                                ])
                                                    )
                                                ]
                                            ]
                                        ]
                                    ]

                                ])
                        )
                    ]
                ]
            ])

    let view () =
        Component(fun ctx ->
            let weatherState = ctx.useState (temperature, false)
            let timelineState = ctx.useState (timeline, false)
            let forecastState = ctx.useState (forecast, false)

            let mockWeatherProvider = TemporaryWeatherProvider()
            let display = AvaloniaWeatherDisplay(weatherState, timelineState, forecastState)
            let timeService = TimeService()
            let logger = ConsoleLogger()

            let app =
                HomeWeatherApplication<celsius>(mockWeatherProvider, display, timeService, logger)

            let callback () =
                async {
                    while true do
                        app.RunAsync() |> Async.AwaitTask |> ignore
                }

            callback () |> Async.Start

            DockPanel.create [
                DockPanel.children [
                    ScrollViewer.create [ ScrollViewer.content (singleDayWeatherComponent weatherState) ]
                    ScrollViewer.create [ ScrollViewer.content (singleTimelineComponent timelineState) ]
                    ScrollViewer.create [ ScrollViewer.content (forecastComponent forecastState) ]
                ]
            ])

type MainWindow() =
    inherit HostWindow()

    do
        base.Title <- "Weather"
        base.Content <- Main.view ()

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add(FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime -> desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main (args: string[]) =
        AppBuilder.Configure<App>().UsePlatformDetect().UseSkia().StartWithClassicDesktopLifetime args
