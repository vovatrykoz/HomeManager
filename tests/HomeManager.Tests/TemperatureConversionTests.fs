namespace HomeManager.Tests

open HomeManager.Core.Weather
open NUnit.Framework
open FsCheck.NUnit

module ``Temperature Conversion Tests`` =
    //
    [<Property>]
    let ``Can correctly convert between celsius and fahrenheit`` (celsiusValue: float32<celsius>) =
        let expected = celsiusValue * 1.8f<fahrenheit / celsius> + 32.0f<fahrenheit>
        let actual = Convert.celsiusToFahrenheit celsiusValue

        Assert.That(actual, Is.EqualTo expected)

    [<Property>]
    let ``Can correctly convert between celsius and kelvin`` (celsiusValue: float32<celsius>) =
        let expected = celsiusValue * 1.0f<kelvin / celsius> + 273.15f<kelvin>
        let actual = Convert.celsiusToKelvin celsiusValue

        Assert.That(actual, Is.EqualTo expected)

    [<Property>]
    let ``Can correctly convert between fahrenheit and celsius`` (fahrenheitValue: float32<fahrenheit>) =
        let expected = (fahrenheitValue - 32.0f<fahrenheit>) / 1.8f<fahrenheit / celsius>
        let actual = Convert.fahrenheitToCelsius fahrenheitValue
        Assert.That(actual, Is.EqualTo expected)

    [<Property>]
    let ``Can correctly convert between fahrenheit and kelvin`` (fahrenheitValue: float32<fahrenheit>) =
        let expected =
            (fahrenheitValue - 32.0f<fahrenheit>) / 1.8f<fahrenheit / kelvin>
            + 273.15f<kelvin>

        let actual = Convert.fahrenheitToKelvin fahrenheitValue
        Assert.That(actual, Is.EqualTo expected)

    [<Property>]
    let ``Can correctly convert between kelvin and celsius`` (kelvinValue: float32<kelvin>) =
        let expected = (kelvinValue - 273.15f<kelvin>) * 1.0f<celsius / kelvin>
        let actual = Convert.kelvinToCelsius kelvinValue
        Assert.That(actual, Is.EqualTo expected)

    [<Property>]
    let ``Can correctly convert between kelvin and fahrenheit`` (kelvinValue: float32<kelvin>) =
        let expected =
            (kelvinValue - 273.15f<kelvin>) * 1.8f<fahrenheit / kelvin> + 32.0f<fahrenheit>

        let actual = Convert.kelvinToFahrenheit kelvinValue
        Assert.That(actual, Is.EqualTo expected)
