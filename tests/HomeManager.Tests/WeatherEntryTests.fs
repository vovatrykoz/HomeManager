namespace HomeManager.Tests

open HomeManager.Core.Weather
open NUnit.Framework
open NUnit.Framework.Legacy
open FsCheck.NUnit
open System

[<Properties(Arbitrary = [| typeof<HomeManagerGen> |])>]
module ``Weather Entry Tests`` =
    //
    [<Property>]
    let ``Can create a weather entry`` (time: TimeOnly) (data: WeatherData<celsius>) =
        let actual = WeatherEntry<celsius>.create time data

        Assert.Multiple(fun () ->
            Assert.That(actual.TimeOfDay, Is.EqualTo time)
            Assert.That(actual.Data, Is.EqualTo data))

[<Properties(Arbitrary = [| typeof<HomeManagerGen> |])>]
module ``Day Weather Tests`` =
    //
    [<Property>]
    let ``Can create a day weather object`` (day: DateOnly) (rawData: WeatherEntry<celsius> array) =
        let actual = DayWeather<celsius>.create day rawData

        Assert.Multiple(fun () ->
            Assert.That(actual.Day, Is.EqualTo day)
            CollectionAssert.AreEqual(rawData, actual.Entries))

    [<Property>]
    let ``Can correctly calculate average temperature using the static member``
        (day: DateOnly)
        (rawData: WeatherEntry<celsius> array)
        =
        if Array.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateAverageTemperature |> ignore)
            |> ignore
        else
            let actual = DayWeather<celsius>.create day rawData
            let actualTemperatureAverage = actual |> DayWeather.calculateAverageTemperature

            let expectedTemperatureAverage =
                rawData |> Array.averageBy (fun entry -> entry.Data.GetTemperature)

            Assert.That(actualTemperatureAverage, Is.EqualTo expectedTemperatureAverage)

    [<Property>]
    let ``Can correctly calculate max temperature using the static member``
        (day: DateOnly)
        (rawData: WeatherEntry<celsius> array)
        =
        if Array.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateMaxTemperature |> ignore)
            |> ignore
        else
            let actual = DayWeather<celsius>.create day rawData
            let actualTemperature = actual |> DayWeather.calculateMaxTemperature
            let expected = rawData |> Array.maxBy (fun entry -> entry.Data.GetTemperature)

            Assert.That(actualTemperature, Is.EqualTo expected.Data.GetTemperature)

    [<Property>]
    let ``Can correctly calculate min temperature using the static member``
        (day: DateOnly)
        (rawData: WeatherEntry<celsius> array)
        =
        if Array.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateMinTemperature |> ignore)
            |> ignore
        else
            let actual = DayWeather<celsius>.create day rawData
            let actualTemperature = actual |> DayWeather.calculateMinTemperature
            let expected = rawData |> Array.minBy (fun entry -> entry.Data.GetTemperature)

            Assert.That(actualTemperature, Is.EqualTo expected.Data.GetTemperature)

    [<Property>]
    let ``Can correctly calculate average precipitation using the static member``
        (day: DateOnly)
        (rawData: WeatherEntry<celsius> array)
        =
        if Array.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateAveragePrecipitation |> ignore)
            |> ignore
        else
            let actual = DayWeather<celsius>.create day rawData
            let actualPrecipitationAverage = actual |> DayWeather.calculateAveragePrecipitation

            let expectedPrecipitationAverage =
                rawData |> Array.averageBy (fun entry -> entry.Data.GetPrecipitation)

            Assert.That(actualPrecipitationAverage, Is.EqualTo expectedPrecipitationAverage)

    [<Property>]
    let ``Can correctly calculate average humidity using the static member``
        (day: DateOnly)
        (rawData: WeatherEntry<celsius> array)
        =
        if Array.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateAverageHumidity |> ignore)
            |> ignore
        else
            let actual = DayWeather<celsius>.create day rawData
            let actualHumidityAverage = actual |> DayWeather.calculateAverageHumidity

            let expectedHumidityAverage =
                rawData |> Array.averageBy (fun entry -> entry.Data.GetHumidity)

            Assert.That(actualHumidityAverage, Is.EqualTo expectedHumidityAverage)

    [<Property>]
    let ``Can correctly calculate average wind speed using the static member``
        (day: DateOnly)
        (rawData: WeatherEntry<celsius> array)
        =
        if Array.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateAverageWindSpeed |> ignore)
            |> ignore
        else
            let actual = DayWeather<celsius>.create day rawData
            let actualWindSpeedAverage = actual |> DayWeather.calculateAverageWindSpeed

            let expectedWindSpeedAverage =
                rawData |> Array.averageBy (fun entry -> entry.Data.GetWindSpeed)

            Assert.That(actualWindSpeedAverage, Is.EqualTo expectedWindSpeedAverage)

    [<Property>]
    let ``Can correctly calculate average temperature using the instance member``
        (day: DateOnly)
        (rawData: WeatherEntry<celsius> array)
        =
        if Array.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateAverageTemperature |> ignore)
            |> ignore
        else
            let actual = DayWeather<celsius>.create day rawData
            let actualTemperatureAverage = actual.CalculateAverageTemperature()

            let expectedTemperatureAverage =
                rawData |> Array.averageBy (fun entry -> entry.Data.GetTemperature)

            Assert.That(actualTemperatureAverage, Is.EqualTo expectedTemperatureAverage)

    [<Property>]
    let ``Can correctly calculate max temperature using the instance member``
        (day: DateOnly)
        (rawData: WeatherEntry<celsius> array)
        =
        if Array.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateMaxTemperature |> ignore)
            |> ignore
        else
            let actual = DayWeather<celsius>.create day rawData
            let actualTemperature = actual.CalculateMaxTemperature()

            let expected =
                rawData |> Array.map (fun entry -> entry.Data.GetTemperature) |> Array.max

            Assert.That(actualTemperature, Is.EqualTo expected)

    [<Property>]
    let ``Can correctly calculate min temperature using the instance member``
        (day: DateOnly)
        (rawData: WeatherEntry<celsius> array)
        =
        if Array.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateMinTemperature |> ignore)
            |> ignore
        else
            let actual = DayWeather<celsius>.create day rawData
            let actualTemperature = actual.CalculateMinTemperature()

            let expected =
                rawData |> Array.map (fun entry -> entry.Data.GetTemperature) |> Array.min

            Assert.That(actualTemperature, Is.EqualTo expected)

    [<Property>]
    let ``Can correctly calculate average precipitation using the instance member``
        (day: DateOnly)
        (rawData: WeatherEntry<celsius> array)
        =
        if Array.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateAveragePrecipitation |> ignore)
            |> ignore
        else
            let actual = DayWeather<celsius>.create day rawData
            let actualPrecipitationAverage = actual.CalculateAveragePrecipitation()

            let expectedPrecipitationAverage =
                rawData |> Array.averageBy (fun entry -> entry.Data.GetPrecipitation)

            Assert.That(actualPrecipitationAverage, Is.EqualTo expectedPrecipitationAverage)

    [<Property>]
    let ``Can correctly calculate average humidity using the instance member``
        (day: DateOnly)
        (rawData: WeatherEntry<celsius> array)
        =
        if Array.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateAverageHumidity |> ignore)
            |> ignore
        else
            let actual = DayWeather<celsius>.create day rawData
            let actualHumidityAverage = actual.CalculateAverageHumidity()

            let expectedHumidityAverage =
                rawData |> Array.averageBy (fun entry -> entry.Data.GetHumidity)

            Assert.That(actualHumidityAverage, Is.EqualTo expectedHumidityAverage)

    [<Property>]
    let ``Can correctly calculate average wind speed using the instance member``
        (day: DateOnly)
        (rawData: WeatherEntry<celsius> array)
        =
        if Array.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual.CalculateAverageWindSpeed() |> ignore)
            |> ignore
        else
            let actual = DayWeather<celsius>.create day rawData
            let actualWindSpeedAverage = actual.CalculateAverageWindSpeed()

            let expectedWindSpeedAverage =
                rawData |> Array.averageBy (fun entry -> entry.Data.GetWindSpeed)

            Assert.That(actualWindSpeedAverage, Is.EqualTo expectedWindSpeedAverage)
