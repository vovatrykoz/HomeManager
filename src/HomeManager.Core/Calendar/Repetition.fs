namespace HomeManager.Core.Calendar

type Repetition = {
    Number: int
    Interval: FixedRepetitionInterval
} with

    [<CompiledName("Create")>]
    static member create number interval = { Number = number; Interval = interval }
