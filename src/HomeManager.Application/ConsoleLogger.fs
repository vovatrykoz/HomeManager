namespace HomeManager.Application

open System

[<Struct>]
type LogLevel =
    | Info
    | Warning
    | Error
    | Debug

type ConsoleLogger() =

    let _lock = obj ()

    member _.Log level message =
        let currentTime = DateTime.Now

        let levelString =
            match level with
            | Info -> "[INFO]"
            | Warning -> "[WARN]"
            | Error -> "[ERROR]"
            | Debug -> "[DEBUG]"

        let completeMessage = $"[{currentTime}] {levelString} {message}"
        lock _lock (fun _ -> Console.WriteLine completeMessage)

    interface ILogger with
        member this.LogDebug(message: string) : unit = this.Log Debug message
        member this.LogError(message: string) : unit = this.Log Error message
        member this.LogInfo(message: string) : unit = this.Log Info message
        member this.LogWarning(message: string) : unit = this.Log Warning message
