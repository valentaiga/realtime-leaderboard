using System.Runtime.Serialization;

namespace BackOffice.Chronicle.Grpc;

[DataContract]
public class GetPlayerMatchesFilter
{
    [DataMember(Order = 1)]
    public GrpcFilterDescriptor<long>? PlayerId { get; set; }

    [DataMember(Order = 2)]
    public GrpcFilterDescriptor<DateTime>? StartedAt { get; set; }

    [DataMember(Order = 3)]
    public GrpcFilterDescriptor<DateTime>? FinishedAt { get; set; }

    [DataMember(Order = 4)]
    public GrpcFilterDescriptor<bool>? PlayerWon { get; set; }

    [DataMember(Order = 5)]
    public long Limit { get; set; }

    [DataMember(Order = 6)]
    public long Offset { get; set; }
}