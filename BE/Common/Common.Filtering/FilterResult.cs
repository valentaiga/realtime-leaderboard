using System.Runtime.Serialization;

namespace Common.Filtering;

[DataContract]
public class FilterResult<TData>
{
    public FilterResult()
    {
    }

    public FilterResult(TData[] data, ulong total)
    {
        Data = data;
        Total = total;
    }

    [DataMember(Order = 1)]
    public IEnumerable<TData> Data { get; set; } = [];

    [DataMember(Order = 2)]
    public ulong Total { get; set; }
}