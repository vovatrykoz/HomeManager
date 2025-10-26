namespace HomeManager.Avalonia

open HomeManager.Core.Weather

type SimpleWeatherPresenter<[<Measure>] 'tempUnit>
    (
        maxTemp: float32<'tempUnit>,
        precipitation: float32<percent>,
        humidity: float32<percent>,
        windSpeed: float32<meters / second>
    ) =

    member val MaxTemperature = maxTemp with get, set

    member val Precipitation = precipitation with get, set

    member val Humidity = humidity with get, set

    member val WindSpeed = windSpeed with get, set
