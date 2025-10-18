namespace HomeManager.Core.Weather

type TemperatureUnit =
    | Celsius of float32<celsius>
    | Fahrenheit of float32<fahrenheit>
    | Kelvin of float32<kelvin>
