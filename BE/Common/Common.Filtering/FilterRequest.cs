using System.Runtime.Serialization;

namespace Common.Filtering;

/// <summary> Filter request from frontend. </summary>
[DataContract]
public class FilterRequest
{
    [DataMember(Order = 1)]
    public List<FilterCriterion> Criteria { get; set; } = [];

    [DataMember(Order = 2)]
    public uint Offset { get; set; }

    [DataMember(Order = 3)]
    public uint Limit { get; set; } = 50;
}

/// <summary> Single filter criterion from request. </summary>
// public record FilterCriterion(string FieldName, FilterOperator Operator, string? Value);
[DataContract]
public class FilterCriterion
{
    public FilterCriterion()
    {
    }

    public FilterCriterion(string fieldName, FilterOperator @operator, string? value)
    {
        FieldName = fieldName;
        Operator = @operator;
        Value = value;
    }

    [DataMember(Order = 1)]
    public string FieldName { get; set; } = null!;

    [DataMember(Order = 2)]
    public FilterOperator Operator { get; set; }

    [DataMember(Order = 3)]
    public string? Value { get; set; }
}