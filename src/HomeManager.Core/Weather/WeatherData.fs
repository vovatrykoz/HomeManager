namespace HomeManager.Core.Weather

open System

type WeatherDataError =
    | PrecipitationOutOfRange
    | HumidityOutOfRange
    | NegativeWindSpeed

type WeatherData<[<Measure>] 'tempUnit> = private {
    Temperature: float32<'tempUnit>
    Precipitation: float32<percent>
    Humidity: float32<percent>
    Wind: float32<meters / second>
} with

    [<CompiledName("Create")>]
    static member create temperature precipitation humidity wind =
        if precipitation < 0.0f<percent> || precipitation > 100.0f<percent> then
            raise (
                ArgumentException(
                    $"Percentages must be between 0 and 100. Given: {precipitation}",
                    nameof precipitation
                )
            )

        if humidity < 0.0f<percent> || humidity > 100.0f<percent> then
            raise (ArgumentException($"Percentages must be between 0 and 100. Given: {humidity}", nameof humidity))

        if wind < 0.0f<meters / second> then
            raise (ArgumentException($"Wind speed must not be negative. Given: {wind}", nameof wind))

        {
            Temperature = temperature
            Precipitation = precipitation
            Humidity = humidity
            Wind = wind
        }

    [<CompiledName("TryCreate")>]
    static member tryCreate temperature precipitation humidity wind =
        if precipitation < 0.0f<percent> || precipitation > 100.0f<percent> then
            Error PrecipitationOutOfRange
        elif humidity < 0.0f<percent> || humidity > 100.0f<percent> then
            Error HumidityOutOfRange
        elif wind < 0.0f<meters / second> then
            Error NegativeWindSpeed
        else
            Ok {
                Temperature = temperature
                Precipitation = precipitation
                Humidity = humidity
                Wind = wind
            }
