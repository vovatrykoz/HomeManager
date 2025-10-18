namespace HomeManager.Core.Calendar

open HomeManager.Core
open System

[<NoComparison>]
type CalendarEvent = {
    Id: CalendarEventId
    Name: string
    TimeInterval: TimeInterval
    Occurrence: Occurrence
    Description: string
} with

    member this.RemainingTime(timeService: ITimeService) =
        CalendarEvent.timeLeftUntil timeService this

    [<CompiledName("Create")>]
    static member create name timeInterval occurrence description = {
        Id = CalendarEventId.generate ()
        Name = name
        TimeInterval = timeInterval
        Occurrence = occurrence
        Description = description
    }

    [<CompiledName("TimeLeftUntil")>]
    static member timeLeftUntil (timeService: ITimeService) event =
        let currentTime = timeService.GetCurrentTime()

        match event.TimeInterval with
        | WholeDay day ->
            let timeLeft = day.ToDateTime(TimeOnly(0, 0, 0)) - currentTime
            max (TimeSpan 0) timeLeft
        | Exact interval -> interval.StartTime - currentTime

    [<CompiledName("Duration")>]
    static member duration event =
        match event.TimeInterval with
        | WholeDay _ -> TimeSpan.FromDays 1
        | Exact interval -> interval.Duration
