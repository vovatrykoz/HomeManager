namespace HomeManager.Tests

open HomeManager.Core.Weather
open NUnit.Framework
open NUnit.Framework.Legacy
open FsCheck.NUnit
open System

module ``Weather Entry Tests`` =
    //
    [<Property>]
    let ``Can create a weather entry`` (time: DateTime) (data: WeatherData<celsius>) =
        let time = TimeOnly.FromDateTime time
        let actual = WeatherEntry<celsius>.create time data

        Assert.Multiple(fun () ->
            Assert.That(actual.TimeOfDay, Is.EqualTo time)
            Assert.That(actual.Data, Is.EqualTo data))

module ``Day Weather Tests`` =
    //
    [<Property>]
    let ``Can create a day weather object`` (time: DateTime) (rawData: (DateTime * WeatherData<celsius>) list) =
        let day = DateOnly.FromDateTime time

        let data =
            rawData
            |> List.map (fun (date, data) -> WeatherEntry<_>.create (TimeOnly.FromDateTime date) data)
            |> List.toSeq

        let actual = DayWeather<celsius>.create day data

        Assert.Multiple(fun () ->
            Assert.That(actual.Day, Is.EqualTo day)
            CollectionAssert.AreEqual(data, actual.Entries))

    [<Property>]
    let ``Can correctly calculate average temperature using the static member``
        (time: DateTime)
        (rawData: (DateTime * WeatherData<celsius>) list)
        =
        let day = DateOnly.FromDateTime time

        if List.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateAverageTemperature |> ignore)
            |> ignore
        else

            let data =
                rawData
                |> List.map (fun (date, data) -> WeatherEntry<_>.create (TimeOnly.FromDateTime date) data)
                |> List.toSeq

            let actual = DayWeather<celsius>.create day data
            let actualTemperatureAverage = actual |> DayWeather.calculateAverageTemperature

            let expectedTemperatureAverage =
                rawData |> List.averageBy (fun (_, rawEntry) -> rawEntry.GetTemperature)

            Assert.That(actualTemperatureAverage, Is.EqualTo expectedTemperatureAverage)

    [<Property>]
    let ``Can correctly calculate max temperature using the static member``
        (time: DateTime)
        (rawData: (DateTime * WeatherData<celsius>) list)
        =
        let day = DateOnly.FromDateTime time

        if List.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateMaxTemperature |> ignore)
            |> ignore
        else

            let data =
                rawData
                |> List.map (fun (date, data) -> WeatherEntry<_>.create (TimeOnly.FromDateTime date) data)
                |> List.toSeq

            let actual = DayWeather<celsius>.create day data
            let actualTemperature = actual |> DayWeather.calculateMaxTemperature

            let _, expected =
                rawData |> List.maxBy (fun (_, rawEntry) -> rawEntry.GetTemperature)

            Assert.That(actualTemperature, Is.EqualTo expected.GetTemperature)

    [<Property>]
    let ``Can correctly calculate min temperature using the static member``
        (time: DateTime)
        (rawData: (DateTime * WeatherData<celsius>) list)
        =
        let day = DateOnly.FromDateTime time

        if List.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateMinTemperature |> ignore)
            |> ignore
        else

            let data =
                rawData
                |> List.map (fun (date, data) -> WeatherEntry<_>.create (TimeOnly.FromDateTime date) data)
                |> List.toSeq

            let actual = DayWeather<celsius>.create day data
            let actualTemperature = actual |> DayWeather.calculateMinTemperature

            let _, expected =
                rawData |> List.minBy (fun (_, rawEntry) -> rawEntry.GetTemperature)

            Assert.That(actualTemperature, Is.EqualTo expected.GetTemperature)

    [<Property>]
    let ``Can correctly calculate average precipitation using the static member``
        (time: DateTime)
        (rawData: (DateTime * WeatherData<celsius>) list)
        =
        let day = DateOnly.FromDateTime time

        if List.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateAveragePrecipitation |> ignore)
            |> ignore
        else

            let data =
                rawData
                |> List.map (fun (date, data) -> WeatherEntry<_>.create (TimeOnly.FromDateTime date) data)
                |> List.toSeq

            let actual = DayWeather<celsius>.create day data
            let actualPrecipitationAverage = actual |> DayWeather.calculateAveragePrecipitation

            let expectedPrecipitationAverage =
                rawData |> List.averageBy (fun (_, rawEntry) -> rawEntry.GetPrecipitation)

            Assert.That(actualPrecipitationAverage, Is.EqualTo expectedPrecipitationAverage)

    [<Property>]
    let ``Can correctly calculate average humidity using the static member``
        (time: DateTime)
        (rawData: (DateTime * WeatherData<celsius>) list)
        =
        let day = DateOnly.FromDateTime time

        if List.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateAverageHumidity |> ignore)
            |> ignore
        else

            let data =
                rawData
                |> List.map (fun (date, data) -> WeatherEntry<_>.create (TimeOnly.FromDateTime date) data)
                |> List.toSeq

            let actual = DayWeather<celsius>.create day data
            let actualHumidityAverage = actual |> DayWeather.calculateAverageHumidity

            let expectedHumidityAverage =
                rawData |> List.averageBy (fun (_, rawEntry) -> rawEntry.GetHumidity)

            Assert.That(actualHumidityAverage, Is.EqualTo expectedHumidityAverage)

    [<Property>]
    let ``Can correctly calculate average wind speed using the static member``
        (time: DateTime)
        (rawData: (DateTime * WeatherData<celsius>) list)
        =
        let day = DateOnly.FromDateTime time

        if List.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual |> DayWeather.calculateAverageWindSpeed |> ignore)
            |> ignore
        else

            let data =
                rawData
                |> List.map (fun (date, data) -> WeatherEntry<_>.create (TimeOnly.FromDateTime date) data)
                |> List.toSeq

            let actual = DayWeather<celsius>.create day data
            let actualWindSpeedAverage = actual |> DayWeather.calculateAverageWindSpeed

            let expectedWindSpeedAverage =
                rawData |> List.averageBy (fun (_, rawEntry) -> rawEntry.GetWindSpeed)

            Assert.That(actualWindSpeedAverage, Is.EqualTo expectedWindSpeedAverage)

    [<Property>]
    let ``Can correctly calculate average temperature using the instance member``
        (time: DateTime)
        (rawData: (DateTime * WeatherData<celsius>) list)
        =
        let day = DateOnly.FromDateTime time

        if List.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual.CalculateAverageTemperature() |> ignore)
            |> ignore
        else

            let data =
                rawData
                |> List.map (fun (date, data) -> WeatherEntry<_>.create (TimeOnly.FromDateTime date) data)
                |> List.toSeq

            let actual = DayWeather<celsius>.create day data
            let actualTemperatureAverage = actual.CalculateAverageTemperature()

            let expectedTemperatureAverage =
                rawData |> List.averageBy (fun (_, rawEntry) -> rawEntry.GetTemperature)

            Assert.That(actualTemperatureAverage, Is.EqualTo expectedTemperatureAverage)

    [<Property>]
    let ``Can correctly calculate max temperature using the instacne member``
        (time: DateTime)
        (rawData: (DateTime * WeatherData<celsius>) list)
        =
        let day = DateOnly.FromDateTime time

        if List.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual.CalculateMaxTemperature() |> ignore)
            |> ignore
        else

            let data =
                rawData
                |> List.map (fun (date, data) -> WeatherEntry<_>.create (TimeOnly.FromDateTime date) data)
                |> List.toSeq

            let actual = DayWeather<celsius>.create day data
            let actualTemperature = actual.CalculateMaxTemperature()

            let _, expected =
                rawData |> List.maxBy (fun (_, rawEntry) -> rawEntry.GetTemperature)

            Assert.That(actualTemperature, Is.EqualTo expected.GetTemperature)

    [<Property>]
    let ``Can correctly calculate min temperature using the instacne member``
        (time: DateTime)
        (rawData: (DateTime * WeatherData<celsius>) list)
        =
        let day = DateOnly.FromDateTime time

        if List.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual.CalculateMinTemperature() |> ignore)
            |> ignore
        else

            let data =
                rawData
                |> List.map (fun (date, data) -> WeatherEntry<_>.create (TimeOnly.FromDateTime date) data)
                |> List.toSeq

            let actual = DayWeather<celsius>.create day data
            let actualTemperature = actual.CalculateMinTemperature()

            let _, expected =
                rawData |> List.minBy (fun (_, rawEntry) -> rawEntry.GetTemperature)

            Assert.That(actualTemperature, Is.EqualTo expected.GetTemperature)

    [<Property>]
    let ``Can correctly calculate average precipitation using the instance member``
        (time: DateTime)
        (rawData: (DateTime * WeatherData<celsius>) list)
        =
        let day = DateOnly.FromDateTime time

        if List.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual.CalculateAveragePrecipitation() |> ignore)
            |> ignore
        else

            let data =
                rawData
                |> List.map (fun (date, data) -> WeatherEntry<_>.create (TimeOnly.FromDateTime date) data)
                |> List.toSeq

            let actual = DayWeather<celsius>.create day data
            let actualPrecipitationAverage = actual.CalculateAveragePrecipitation()

            let expectedPrecipitationAverage =
                rawData |> List.averageBy (fun (_, rawEntry) -> rawEntry.GetPrecipitation)

            Assert.That(actualPrecipitationAverage, Is.EqualTo expectedPrecipitationAverage)

    [<Property>]
    let ``Can correctly calculate average humidity using the instance member``
        (time: DateTime)
        (rawData: (DateTime * WeatherData<celsius>) list)
        =
        let day = DateOnly.FromDateTime time

        if List.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual.CalculateAverageHumidity() |> ignore)
            |> ignore
        else

            let data =
                rawData
                |> List.map (fun (date, data) -> WeatherEntry<_>.create (TimeOnly.FromDateTime date) data)
                |> List.toSeq

            let actual = DayWeather<celsius>.create day data
            let actualHumidityAverage = actual.CalculateAverageHumidity()

            let expectedHumidityAverage =
                rawData |> List.averageBy (fun (_, rawEntry) -> rawEntry.GetHumidity)

            Assert.That(actualHumidityAverage, Is.EqualTo expectedHumidityAverage)

    [<Property>]
    let ``Can correctly calculate average wind speed using the instance member``
        (time: DateTime)
        (rawData: (DateTime * WeatherData<celsius>) list)
        =
        let day = DateOnly.FromDateTime time

        if List.isEmpty rawData then
            let actual = DayWeather<celsius>.create day Seq.empty

            Assert.Throws<ArgumentException>(fun () -> actual.CalculateAverageWindSpeed() |> ignore)
            |> ignore
        else

            let data =
                rawData
                |> List.map (fun (date, data) -> WeatherEntry<_>.create (TimeOnly.FromDateTime date) data)
                |> List.toSeq

            let actual = DayWeather<celsius>.create day data
            let actualWindSpeedAverage = actual.CalculateAverageWindSpeed()

            let expectedWindSpeedAverage =
                rawData |> List.averageBy (fun (_, rawEntry) -> rawEntry.GetWindSpeed)

            Assert.That(actualWindSpeedAverage, Is.EqualTo expectedWindSpeedAverage)
