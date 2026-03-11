using System.Runtime.Serialization;

namespace BackOffice.Chronicle.Grpc;

[DataContract]
public class GrpcMatchInfo
{
    [DataMember(Order = 1)]
    public string MatchId { get; set; } = null!;
    
    [DataMember(Order = 2)]
    public List<GrpcMatchPlayer> Players { get; set; } = [];

    [DataMember(Order = 3)]
    public DateTime StartedAt { get; set; }

    [DataMember(Order = 4)]
    public DateTime FinishedAt { get; set; }
}