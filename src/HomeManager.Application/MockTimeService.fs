namespace HomeManager.Application

open HomeManager.Core
open System

type MockTimeService() =
    interface ITimeService with
        member this.GetCurrentTime() = DateTime.Now
