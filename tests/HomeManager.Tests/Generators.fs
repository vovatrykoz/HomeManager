namespace HomeManager.Tests

open HomeManager.Core.Weather
open FsCheck.FSharp
open System

type ValidPercentage(value: float32<percent>) =
    member _.Get = value

type ValidSpeed(value: float32<meters / second>) =
    member _.Get = value

type InvalidPercentage(value: float32<percent>) =
    member _.Get = value

type InvalidSpeed(value: float32<meters / second>) =
    member _.Get = value

type InDateRangeForecast(startTime: DateTime, endTime: DateTime, forecast: DayWeather<celsius> array) =

    member _.StartTime = startTime

    member _.EndTime = endTime

    member _.Forecast = forecast

module Generators =
    //
    module Units =
        //
        let validPercentageGen () =
            ArbMap.defaults.ArbFor<float32<percent>>()
            |> Arb.toGen
            |> Gen.filter (fun x -> x >= 0.0f<percent> && x <= 100.0f<percent>)

        let validPercentageArb () =
            validPercentageGen () |> Gen.map (fun x -> ValidPercentage x) |> Arb.fromGen

        let validSpeedGen () =
            ArbMap.defaults.ArbFor<float32<meters / second>>()
            |> Arb.toGen
            |> Gen.filter (fun x -> x >= 0.0f<meters / second>)

        let validSpeedArb () =
            validSpeedGen () |> Gen.map (fun x -> ValidSpeed x) |> Arb.fromGen

        let invalidPercentageGen () =
            ArbMap.defaults.ArbFor<float32<percent>>()
            |> Arb.toGen
            |> Gen.filter (fun x -> x < 0.0f<percent> || x > 100.0f<percent>)

        let invalidPercentageArb () =
            invalidPercentageGen () |> Gen.map (fun x -> InvalidPercentage x) |> Arb.fromGen

        let invalidSpeedGen () =
            ArbMap.defaults.ArbFor<float32<meters / second>>()
            |> Arb.toGen
            |> Gen.filter (fun x -> x < 0.0f<meters / second>)

        let invalidSpeedArb () =
            invalidSpeedGen () |> Gen.map (fun x -> InvalidSpeed x) |> Arb.fromGen

    module Time =
        open System

        let timeOnlyGen () =
            ArbMap.defaults.ArbFor<DateTime>()
            |> Arb.toGen
            |> Gen.map (fun d -> TimeOnly.FromDateTime d)

        let timeOnlyArb () = timeOnlyGen () |> Arb.fromGen

        let dateOnlyGen () =
            ArbMap.defaults.ArbFor<DateTime>()
            |> Arb.toGen
            |> Gen.map (fun d -> DateOnly.FromDateTime d)

        let dateOnlyArb () = dateOnlyGen () |> Arb.fromGen

    module Forecast =
        open System

        let inDateRangeForecastGen () =
            let mergedMap =
                ArbMap.defaults
                |> ArbMap.mergeArb (Time.dateOnlyArb ())
                |> ArbMap.mergeArb (Time.timeOnlyArb ())

            mergedMap.ArbFor<DateTime * DateTime * DayWeather<celsius> array>()
            |> Arb.toGen
            |> Gen.filter (fun (startTime, endTime, data) ->
                endTime > startTime
                && data
                   |> Array.forall (fun e ->
                       let exactTime = e.Day.ToDateTime(TimeOnly.FromDateTime startTime)
                       exactTime >= startTime && exactTime <= endTime))

        let inDateRangeForecastArb () =
            inDateRangeForecastGen ()
            |> Gen.map (fun (startTime, endTime, data) -> InDateRangeForecast(startTime, endTime, data))
            |> Arb.fromGen

        let inRangeManualGen (startTime: DateTime) (endTime: DateTime) =
            ArbMap.defaults.ArbFor<DateTime * DayWeather<celsius> array>()
            |> Arb.toGen
            |> Gen.filter (fun (randomTime, entries) ->
                entries
                |> Array.forall (fun e ->
                    let exactTime = DateTime(e.Day, TimeOnly.FromDateTime randomTime)
                    exactTime >= startTime && exactTime <= endTime))
            |> Gen.map (fun (_, data) -> data)

type HomeManagerGen() =

    static member ValidPercentage() = Generators.Units.validPercentageArb ()

    static member ValidSpeed() = Generators.Units.validSpeedArb ()

    static member InvalidPercentage() =
        Generators.Units.invalidPercentageArb ()

    static member InvalidSpeed() = Generators.Units.invalidSpeedArb ()

    static member TimeOnly() = Generators.Time.timeOnlyArb ()

    static member DateOnly() = Generators.Time.dateOnlyArb ()

    static member InDateRangeForecast() =
        Generators.Forecast.inDateRangeForecastArb ()
