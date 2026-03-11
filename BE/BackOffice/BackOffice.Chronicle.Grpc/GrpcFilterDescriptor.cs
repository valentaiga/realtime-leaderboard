using System.Runtime.Serialization;
using Common.Filtering;

namespace BackOffice.Chronicle.Grpc;

[DataContract]
public class GrpcFilterDescriptor<TValue>
{
    [DataMember(Order = 1)]
    public TValue? Value { get; set; }

    [DataMember(Order = 2)]
    public FilterOperator Operator { get; set; }
}