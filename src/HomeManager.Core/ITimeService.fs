namespace HomeManager.Core

open System

[<Interface>]
type ITimeService =
    abstract member GetCurrentTime: unit -> DateTime
