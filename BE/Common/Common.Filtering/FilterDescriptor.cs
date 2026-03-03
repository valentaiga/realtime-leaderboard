namespace Common.Filtering;

public sealed class FilterDescriptor<TValue>(TValue? value, FilterOperator filterOperator)
{
    public TValue? Value { get; set; } = value;

    public FilterOperator Operator { get; set; } = filterOperator;
}