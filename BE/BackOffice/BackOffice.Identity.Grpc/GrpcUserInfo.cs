using System.Runtime.Serialization;

namespace BackOffice.Identity.Grpc;

[DataContract]
public class GrpcUserInfo
{
    [DataMember(Order = 1)]
    public long UserId { get; set; }

    [DataMember(Order = 2)]
    public string UserName { get; set; } = null!;
}