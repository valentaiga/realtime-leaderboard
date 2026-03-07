using System.Runtime.Serialization;
using System.ServiceModel;

namespace BackOffice.Identity.Grpc;

[ServiceContract]
public interface IIdentityApi
{
    [OperationContract]
    Task<GrpcChallengeUserResponse> ChallengeUser(GrpcChallengeUserRequest request, CancellationToken ct);

    [OperationContract]
    Task<GrpcGetUserByIdResponse> GetUserById(GrpcGetUserByIdRequest userId, CancellationToken ct);
}

[DataContract]
public class GrpcChallengeUserRequest
{
    [DataMember(Order = 1)]
    public string Username { get; set; } = null!;

    [DataMember(Order = 2)]
    public string Password { get; set; } = null!;
}

[DataContract]
public class GrpcChallengeUserResponse(GrpcUserInfo User)
{
    [DataMember(Order = 1)]
    public GrpcUserInfo User { get; set; } = null!;
}

[DataContract]
public class GrpcUserInfo
{
    [DataMember(Order = 1)]
    public long UserId { get; set; }

    [DataMember(Order = 2)]
    public string UserName { get; set; } = null!;
}

[DataContract]
public class GrpcGetUserByIdRequest
{
    [DataMember(Order = 1)]
    public long UserId { get; set; }
}

[DataContract]
public class GrpcGetUserByIdResponse
{
    [DataMember(Order = 1)]
    public GrpcUserInfo User { get; set; } = null!;
}
