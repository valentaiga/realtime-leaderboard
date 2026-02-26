namespace Common.Filtering;

public enum FilterOperator : byte
{
    Equals = 0,
    NotEquals = 1,
    GreaterThan = 2,
    GreaterThanOrEqual = 3,
    LessThan = 4,
    LessThanOrEqual = 5,
    Contains = 6,
    NotContains = 7,
    In = 8,
    NotIn = 9,
    Between = 10,
    StartsWith = 11,
    EndsWith = 12
}