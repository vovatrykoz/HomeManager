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

    let temperature =
        SimpleWeatherPresenter(0.0f<celsius>, 50.0f<percent>, 50.0f<percent>, 3.0f<meters / second>)

    let rec apiCallback (app: HomeWeatherApplication<celsius>) = app.RunAsync()

    let otherComponent (state: IWritable<SimpleWeatherPresenter<celsius>>) =
        Component(fun ctx ->
            let state = ctx.usePassed state

            TextBlock.create [
                TextBlock.dock Dock.Top
                TextBlock.fontSize 48.0
                TextBlock.verticalAlignment VerticalAlignment.Center
                TextBlock.horizontalAlignment HorizontalAlignment.Center
                TextBlock.text $"Temperature: {state.Current.MaxTemperature} °C"
            ])

    let view () =
        Component(fun ctx ->
            let state = ctx.useState (temperature, false)
            let mockWeatherProvider = TemporaryWeatherProvider()
            let display = AvaloniaWeatherDisplay state
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

            ContentControl.create [ ContentControl.content (otherComponent state) ])

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
