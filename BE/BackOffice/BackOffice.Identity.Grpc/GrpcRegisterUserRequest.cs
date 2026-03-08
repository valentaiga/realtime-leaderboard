using System.Runtime.Serialization;

namespace BackOffice.Identity.Grpc;

[DataContract]
public class GrpcRegisterUserRequest
{
    [DataMember(Order = 1)]
    public long Id { get; set; }
    
    [DataMember(Order = 2)]
    public string Username { get; set; } = null!;
    
    [DataMember(Order = 3)]
    public string Password { get; set; } = null!;
}