namespace HomeManager.Application

[<Interface>]
type ILogger =
    abstract member LogInfo: message: string -> unit

    abstract member LogWarning: message: string -> unit

    abstract member LogError: message: string -> unit

    abstract member LogDebug: message: string -> unit
