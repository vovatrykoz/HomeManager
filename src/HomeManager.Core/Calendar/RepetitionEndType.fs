namespace HomeManager.Core.Calendar

open System

type RepetitionEndType =
    internal
    | Never
    | AfterDate of date: DateTime
    | AfterRepetitions of repetitions: int
