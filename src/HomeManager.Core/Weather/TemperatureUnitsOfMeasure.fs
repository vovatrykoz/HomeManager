namespace HomeManager.Core.Weather

[<Measure>]
type celsius

[<Measure>]
type fahrenheit

[<Measure>]
type kelvin

module internal Convert =

    let celsiusToFahrenheit (value: float32<celsius>) =
        let v = float value
        (v * 1.8 + 32.0) * 1.0<fahrenheit>

    let fahrenheitToCelsius (value: float32<fahrenheit>) =
        let v = float value
        (v - 32.0) / 1.8 * 1.0<celsius>

    let celsiusToKelvin (value: float32<celsius>) =
        let v = float value
        (v + 273.15) * 1.0<kelvin>

    let kelvinToCelsius (value: float32<kelvin>) =
        let v = float value
        (v - 273.15) * 1.0<celsius>

    let fahrenheitToKelvin (value: float32<fahrenheit>) =
        let v = float value
        ((v - 32.0) / 1.8 + 273.15) * 1.0<kelvin>

    let kelvinToFahrenheit (value: float32<kelvin>) =
        let v = float value
        ((v - 273.15) * 1.8 + 32.0) * 1.0<fahrenheit>
