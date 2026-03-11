using System.Runtime.Serialization;

namespace BackOffice.Chronicle.Grpc;

[DataContract]
public class GrpcMatchPlayer
{
    [DataMember(Order = 1)]
    public long PlayerId { get; set; }

    [DataMember(Order = 2)]
    public bool IsWin { get; set; }

    [DataMember(Order = 3)]
    public int EloChange { get; set; }
}