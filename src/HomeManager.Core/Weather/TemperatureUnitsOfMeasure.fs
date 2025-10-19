namespace HomeManager.Core.Weather

[<Measure>]
type celsius

[<Measure>]
type fahrenheit

[<Measure>]
type kelvin

module internal Convert =

    let celsiusToFahrenheit (value: float32<celsius>) =
        let v = float32 value
        (v * 1.8f + 32.0f) * 1.0f<fahrenheit>

    let fahrenheitToCelsius (value: float32<fahrenheit>) =
        let v = float32 value
        (v - 32.0f) / 1.8f * 1.0f<celsius>

    let celsiusToKelvin (value: float32<celsius>) =
        let v = float32 value
        (v + 273.15f) * 1.0f<kelvin>

    let kelvinToCelsius (value: float32<kelvin>) =
        let v = float32 value
        (v - 273.15f) * 1.0f<celsius>

    let fahrenheitToKelvin (value: float32<fahrenheit>) =
        let v = float32 value
        ((v - 32.0f) / 1.8f + 273.15f) * 1.0f<kelvin>

    let kelvinToFahrenheit (value: float32<kelvin>) =
        let v = float32 value
        ((v - 273.15f) * 1.8f + 32.0f) * 1.0f<fahrenheit>
