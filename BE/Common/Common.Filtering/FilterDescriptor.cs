using System.Text.Json.Serialization;

namespace Common.Filtering;

public sealed class FilterDescriptor<TValue>
{
    [JsonConstructor]
    public FilterDescriptor()
    {
    }

    public FilterDescriptor(TValue? value, FilterOperator filterOperator)
    {
        Value = value;
        Operator = filterOperator;
    }

    public TValue? Value { get; set; }

    public FilterOperator Operator { get; set; }
}