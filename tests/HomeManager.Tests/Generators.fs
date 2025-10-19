namespace HomeManager.Tests

open HomeManager.Core.Weather
open FsCheck.FSharp

type ValidPercentage(value: float32<percent>) =
    member _.Get = value

type ValidSpeed(value: float32<meters / second>) =
    member _.Get = value

type InvalidPercentage(value: float32<percent>) =
    member _.Get = value

type InvalidSpeed(value: float32<meters / second>) =
    member _.Get = value

module internal Generators =
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

type HomeManagerGen() =

    static member ValidPercentage() = Generators.Units.validPercentageArb ()

    static member ValidSpeed() = Generators.Units.validSpeedArb ()

    static member InvalidPercentage() =
        Generators.Units.invalidPercentageArb ()

    static member InvalidSpeed() = Generators.Units.invalidSpeedArb ()
