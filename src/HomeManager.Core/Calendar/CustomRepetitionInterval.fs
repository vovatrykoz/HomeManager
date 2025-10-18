namespace HomeManager.Core.Calendar

[<NoComparison>]
type CustomRepetitionInterval = {
    Repetition: Repetition
    Weekdays: Set<Weekday>
    RepetitionEnd: RepetitionEnd
} with

    [<CompiledName("Create")>]
    static member create repetition weekdays repetitionEnd = {
        Repetition = repetition
        Weekdays = weekdays
        RepetitionEnd = repetitionEnd
    }
