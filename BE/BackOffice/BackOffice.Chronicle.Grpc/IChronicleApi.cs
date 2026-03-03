using System.Runtime.Serialization;
using System.ServiceModel;
using Common.Filtering;
using ProtoBuf;

[module: CompatibilityLevel(CompatibilityLevel.Level300)] // configures how Guid, DateTime etc are handled
namespace BackOffice.Chronicle.Grpc;

[ServiceContract]
public interface IChronicleApi
{
    [OperationContract]
    Task<FilterResult<MatchInfo>> GetPlayerMatches(GetPlayerMatchesFilter request, CancellationToken ct);
}

[DataContract]
public class GetPlayerMatchesFilter
{
    [DataMember(Order = 1)]
    public GrpcFilterDescriptor<ulong>? PlayerId { get; set; }
    
    [DataMember(Order = 2)]
    public GrpcFilterDescriptor<DateTime>? StartedAt { get; set; }
    
    [DataMember(Order = 3)]
    public GrpcFilterDescriptor<DateTime>? FinishedAt { get; set; }

    [DataMember(Order = 4)]
    public uint Limit { get; set; }

    [DataMember(Order = 5)]
    public uint Offset { get; set; }
}

[DataContract]
public class GrpcFilterDescriptor<TValue>
{
    [DataMember(Order = 1)]
    public TValue? Value { get; set; }

    [DataMember(Order = 2)]
    public FilterOperator Operator { get; set; }
}

[DataContract]
public class MatchInfo
{
    [DataMember(Order = 1)]
    public Guid MatchId { get; set; }
    
    [DataMember(Order = 2)]
    public ulong[] Winners { get; set; } = null!;
    
    [DataMember(Order = 3)]
    public ulong[] Losers { get; set; } = null!;
    
    [DataMember(Order = 4)]
    public DateTime StartedAt { get; set; }
    
    [DataMember(Order = 5)]
    public DateTime FinishedAt { get; set; }
}