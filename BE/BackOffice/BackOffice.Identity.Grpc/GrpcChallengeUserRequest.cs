using System.Runtime.Serialization;

namespace BackOffice.Identity.Grpc;

[DataContract]
public class GrpcChallengeUserRequest
{
    [DataMember(Order = 1)]
    public string Username { get; set; } = null!;

    [DataMember(Order = 2)]
    public string Password { get; set; } = null!;
}