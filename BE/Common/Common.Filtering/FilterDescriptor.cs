namespace Common.Filtering;

public class FilterDescriptor<TValue>(TValue? value, FilterOperator filterOperator)
{
    public TValue? Value { get; set; } = value;

    public FilterOperator Operator { get; set; } = filterOperator;
}