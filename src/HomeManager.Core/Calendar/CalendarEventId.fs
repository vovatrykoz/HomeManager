namespace HomeManager.Core.Calendar

open System

type CalendarEventId = {
    Value: Guid
} with

    [<CompiledName("Generate")>]
    static member generate() = { Value = Guid.NewGuid() }
