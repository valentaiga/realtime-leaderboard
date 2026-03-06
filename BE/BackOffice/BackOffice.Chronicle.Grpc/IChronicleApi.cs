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
    Task<FilterResult<GrpcMatchInfo>> GetPlayerMatches(GetPlayerMatchesFilter request, CancellationToken ct);
}

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

[DataContract]
public class GrpcFilterDescriptor<TValue>
{
    [DataMember(Order = 1)]
    public TValue? Value { get; set; }

    [DataMember(Order = 2)]
    public FilterOperator Operator { get; set; }
}

[DataContract]
public class GrpcMatchInfo
{
    [DataMember(Order = 1)]
    public Guid MatchId { get; set; }
    
    [DataMember(Order = 2)]
    public List<GrpcMatchPlayer> Players { get; set; } = [];

    [DataMember(Order = 3)]
    public DateTime StartedAt { get; set; }

    [DataMember(Order = 4)]
    public DateTime FinishedAt { get; set; }
}

[DataContract]
public class GrpcMatchPlayer
{
    [DataMember(Order = 1)]
    public long PlayerId { get; set; }

    [DataMember(Order = 2)]
    public bool IsWin { get; set; }
}