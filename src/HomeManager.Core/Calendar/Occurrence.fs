namespace HomeManager.Core.Calendar

[<NoComparison>]
type Occurrence =
    | Fixed of FixedRepetitionInterval
    | Custom of CustomRepetitionInterval
