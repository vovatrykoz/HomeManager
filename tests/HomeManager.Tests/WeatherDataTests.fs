namespace HomeManager.Tests

open HomeManager.Core.Weather
open NUnit.Framework
open FsCheck.NUnit
open System

[<Properties(Arbitrary = [| typeof<HomeManagerGen> |])>]
module ``Weather Data Tests`` =
    //
    [<Property>]
    let ``Weather data can be constructed from correct arbitrary values using the "create" method``
        (temperature: float32<celsius>)
        (validPrecipitation: ValidPercentage)
        (validHumidity: ValidPercentage)
        (validWindSpeed: ValidSpeed)
        =
        let actual =
            WeatherData<celsius>.create temperature validPrecipitation.Get validHumidity.Get validWindSpeed.Get

        Assert.Multiple(fun () ->
            Assert.That(actual.GetTemperature, Is.EqualTo temperature)
            Assert.That(actual.GetPrecipitation, Is.EqualTo validPrecipitation.Get)
            Assert.That(actual.GetHumidity, Is.EqualTo validHumidity.Get)
            Assert.That(actual.GetWindSpeed, Is.EqualTo validWindSpeed.Get))

    [<Property>]
    let ``Weather data can be constructed from correct arbitrary values using the "tryCreate" method``
        (temperature: float32<celsius>)
        (validPrecipitation: ValidPercentage)
        (validHumidity: ValidPercentage)
        (validWindSpeed: ValidSpeed)
        =
        let actual =
            WeatherData<celsius>.tryCreate temperature validPrecipitation.Get validHumidity.Get validWindSpeed.Get

        match actual with
        | Ok data ->
            Assert.Multiple(fun () ->
                Assert.That(data.GetTemperature, Is.EqualTo temperature)
                Assert.That(data.GetPrecipitation, Is.EqualTo validPrecipitation.Get)
                Assert.That(data.GetHumidity, Is.EqualTo validHumidity.Get)
                Assert.That(data.GetWindSpeed, Is.EqualTo validWindSpeed.Get))
        | Error err -> Assert.Fail $"Expected Ok, but got: {err}"

    [<Property>]
    let ``Weather data "create" method throws when trying to set invalid precipitation``
        (temperature: float32<celsius>)
        (invalidPrecipitation: InvalidPercentage)
        (validHumidity: ValidPercentage)
        (validWindSpeed: ValidSpeed)
        =
        Assert.Throws<ArgumentException>(fun () ->
            WeatherData<celsius>.create temperature invalidPrecipitation.Get validHumidity.Get validWindSpeed.Get
            |> ignore)
        |> ignore

    [<Property>]
    let ``Weather data "tryCreate" returns an error when trying to set invalid precipitation``
        (temperature: float32<celsius>)
        (invalidPrecipitation: InvalidPercentage)
        (validHumidity: ValidPercentage)
        (validWindSpeed: ValidSpeed)
        =
        let actual =
            WeatherData<celsius>.tryCreate temperature invalidPrecipitation.Get validHumidity.Get validWindSpeed.Get

        let expected =
            Result<WeatherData<celsius>, WeatherDataError>.Error PrecipitationOutOfRange

        Assert.That(actual, Is.EqualTo expected)

    [<Property>]
    let ``Weather data "create" method throws when trying to set invalid humidity``
        (temperature: float32<celsius>)
        (validPrecipitation: ValidPercentage)
        (invalidHumidity: InvalidPercentage)
        (validWindSpeed: ValidSpeed)
        =
        Assert.Throws<ArgumentException>(fun () ->
            WeatherData<celsius>.create temperature validPrecipitation.Get invalidHumidity.Get validWindSpeed.Get
            |> ignore)
        |> ignore

    [<Property>]
    let ``Weather data "tryCreate" returns an error when trying to set invalid humidity``
        (temperature: float32<celsius>)
        (validPrecipitation: ValidPercentage)
        (invalidHumidity: InvalidPercentage)
        (validWindSpeed: ValidSpeed)
        =
        let actual =
            WeatherData<celsius>.tryCreate temperature validPrecipitation.Get invalidHumidity.Get validWindSpeed.Get

        let expected =
            Result<WeatherData<celsius>, WeatherDataError>.Error HumidityOutOfRange

        Assert.That(actual, Is.EqualTo expected)

    [<Property>]
    let ``Weather data "create" method throws when trying to set invalid wind speed``
        (temperature: float32<celsius>)
        (validPrecipitation: ValidPercentage)
        (validHumidity: ValidPercentage)
        (invalidWindSpeed: InvalidSpeed)
        =
        Assert.Throws<ArgumentException>(fun () ->
            WeatherData<celsius>.create temperature validPrecipitation.Get validHumidity.Get invalidWindSpeed.Get
            |> ignore)
        |> ignore

    [<Property>]
    let ``Weather data "tryCreate" returns an error when trying to set invalid wind speed``
        (temperature: float32<celsius>)
        (validPrecipitation: ValidPercentage)
        (validHumidity: ValidPercentage)
        (invalidWindSpeed: InvalidSpeed)
        =
        let actual =
            WeatherData<celsius>.tryCreate temperature validPrecipitation.Get validHumidity.Get invalidWindSpeed.Get

        let expected =
            Result<WeatherData<celsius>, WeatherDataError>.Error NegativeWindSpeed

        Assert.That(actual, Is.EqualTo expected)

    [<Property>]
    let ``Weather data can be constructed from arbitrary values using the "create" method``
        (temperature: float32<celsius>)
        (precipitation: float32<percent>)
        (humidity: float32<percent>)
        (windSpeed: float32<meters / second>)
        =
        if precipitation < 0.0f<percent> || precipitation > 100.0f<percent> then
            Assert.Throws<ArgumentException>(fun () ->
                WeatherData<celsius>.create temperature precipitation humidity windSpeed
                |> ignore)
            |> ignore
        elif humidity < 0.0f<percent> || humidity > 100.0f<percent> then
            Assert.Throws<ArgumentException>(fun () ->
                WeatherData<celsius>.create temperature precipitation humidity windSpeed
                |> ignore)
            |> ignore
        elif windSpeed < 0.0f<meters / second> then
            Assert.Throws<ArgumentException>(fun () ->
                WeatherData<celsius>.create temperature precipitation humidity windSpeed
                |> ignore)
            |> ignore
        else
            let actual =
                WeatherData<celsius>.create temperature precipitation humidity windSpeed

            Assert.Multiple(fun () ->
                Assert.That(actual.GetTemperature, Is.EqualTo temperature)
                Assert.That(actual.GetPrecipitation, Is.EqualTo precipitation)
                Assert.That(actual.GetHumidity, Is.EqualTo humidity)
                Assert.That(actual.GetWindSpeed, Is.EqualTo windSpeed))

    [<Property>]
    let ``Weather data can be constructed from arbitrary values using the "tryCreate" method``
        (temperature: float32<celsius>)
        (precipitation: float32<percent>)
        (humidity: float32<percent>)
        (windSpeed: float32<meters / second>)
        =
        if precipitation < 0.0f<percent> || precipitation > 100.0f<percent> then
            let actual =
                WeatherData<celsius>.tryCreate temperature precipitation humidity windSpeed

            let expected =
                Result<WeatherData<celsius>, WeatherDataError>.Error PrecipitationOutOfRange

            Assert.That(actual, Is.EqualTo expected)
        elif humidity < 0.0f<percent> || humidity > 100.0f<percent> then
            let actual =
                WeatherData<celsius>.tryCreate temperature precipitation humidity windSpeed

            let expected =
                Result<WeatherData<celsius>, WeatherDataError>.Error HumidityOutOfRange

            Assert.That(actual, Is.EqualTo expected)
        elif windSpeed < 0.0f<meters / second> then
            let actual =
                WeatherData<celsius>.tryCreate temperature precipitation humidity windSpeed

            let expected =
                Result<WeatherData<celsius>, WeatherDataError>.Error NegativeWindSpeed

            Assert.That(actual, Is.EqualTo expected)
        else
            let actual =
                WeatherData<celsius>.tryCreate temperature precipitation humidity windSpeed

            match actual with
            | Ok data ->
                Assert.Multiple(fun () ->
                    Assert.That(data.GetTemperature, Is.EqualTo temperature)
                    Assert.That(data.GetPrecipitation, Is.EqualTo precipitation)
                    Assert.That(data.GetHumidity, Is.EqualTo humidity)
                    Assert.That(data.GetWindSpeed, Is.EqualTo windSpeed))
            | Error err -> Assert.Fail $"Expected Ok, but got: {err}"
