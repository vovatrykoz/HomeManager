namespace HomeManager.Core.Calendar

open System

type TimeInterval =
    | Exact of ExactTimeInterval
    | WholeDay of DateOnly
