namespace HomeManager.Core.Calendar

open System

type RepetitionEnd =
    | Never
    | AfterDate of Date: DateTime
    | AfterRepetitions of Repetitions: int
