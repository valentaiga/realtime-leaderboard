using System.Runtime.Serialization;

namespace BackOffice.PlayerSearch.Grpc;

[DataContract]
public class GrpcPlayer
{
    [DataMember(Order = 1)]
    public long PlayerId { get; set; }

    [DataMember(Order = 2)]
    public string Username { get; set; } = null!;
}