namespace HomeManager.Application

open HomeManager.Core
open System

type TimeService() =
    interface ITimeService with
        member _.GetCurrentTime() = DateTime.Now
