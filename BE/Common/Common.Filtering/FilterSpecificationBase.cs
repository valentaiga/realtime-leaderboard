using System.Runtime.Serialization;

namespace Common.Filtering;

public abstract class FilterSpecificationBase<T> : IFilterSpecification<T>
{
    private readonly List<FilterCriteria> _criteria = new(4);

    public IReadOnlyList<FilterCriteria> Criteria => _criteria.AsReadOnly();
    public uint Limit { get; set; }
    public uint Offset { get; set; }

    protected void AddCriteria(string fieldName, FilterOperator op, string? value)
    {
        _criteria.Add(new FilterCriteria(fieldName, op, value));
    }

    [DataContract]
    public class FilterCriteria
    {
        public FilterCriteria()
        {
            FieldName = null!;
        }

        public FilterCriteria(string fieldName, FilterOperator @operator, string? value)
        {
            FieldName = fieldName;
            Operator = @operator;
            Value = value;
        }

        [DataMember(Order = 1)]
        public string FieldName { get; set; }

        [DataMember(Order = 2)]
        public FilterOperator Operator { get; set; }

        [DataMember(Order = 3)]
        public string? Value { get; set; }
    }
}

/// <summary> Marker interface for type-safe specification access. </summary>
public interface IFilterSpecification<T>
{
    IReadOnlyList<FilterSpecificationBase<T>.FilterCriteria> Criteria { get; }
}