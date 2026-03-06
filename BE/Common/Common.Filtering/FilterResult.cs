using System.Runtime.Serialization;

namespace Common.Filtering;

[DataContract]
public class FilterResult<TData>
{
    public FilterResult()
    {
    }

    public FilterResult(TData[] data, long total)
    {
        Data = data;
        Total = total;
    }

    [DataMember(Order = 1)]
    public IEnumerable<TData> Data { get; set; } = [];

    [DataMember(Order = 2)]
    public long Total { get; set; }
}