using System.Runtime.Serialization;

namespace BackOffice.Identity.Grpc;

[DataContract]
public class GrpcChallengeUserResponse
{
    [DataMember(Order = 1)]
    public GrpcUserInfo User { get; set; } = null!;
}