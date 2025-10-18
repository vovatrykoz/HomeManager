namespace HomeManager.Core.Calendar

open System

type ExactTimeIntervalError = | StartAfterEnd

type ExactTimeInterval = private {
    StartTime: DateTime
    EndTime: DateTime
} with

    member this.Duration = this.EndTime - this.StartTime

    [<CompiledName("Create")>]
    static member create startTime endTime =
        if endTime < startTime then
            raise (
                ArgumentException(
                    "Start time cannot be set to a time before the end time. "
                    + $"Given start time: {startTime}. Given end time {endTime}"
                )
            )

        {
            StartTime = startTime
            EndTime = endTime
        }

    [<CompiledName("TryCreate")>]
    static member tryCreate startTime endTime =
        if endTime < startTime then
            Error StartAfterEnd
        else
            Ok {
                StartTime = startTime
                EndTime = endTime
            }
