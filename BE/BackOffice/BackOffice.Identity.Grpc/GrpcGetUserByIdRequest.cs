using System.Runtime.Serialization;

namespace BackOffice.Identity.Grpc;

[DataContract]
public class GrpcGetUserByIdRequest
{
    [DataMember(Order = 1)]
    public long UserId { get; set; }
}