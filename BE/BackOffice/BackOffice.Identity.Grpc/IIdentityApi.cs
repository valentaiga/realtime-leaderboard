using System.ServiceModel;

namespace BackOffice.Identity.Grpc;

[ServiceContract]
public interface IIdentityApi // todo vm: update models with [ProtoContract] and other attributes for code consistency
{
    [OperationContract]
    Task<GrpcChallengeUserResponse> ChallengeUser(GrpcChallengeUserRequest request, CancellationToken ct);

    [OperationContract]
    Task<GrpcGetUserByIdResponse> GetUserById(GrpcGetUserByIdRequest userId, CancellationToken ct);
}

public record GrpcChallengeUserRequest(string Username, string Password);
public record GrpcChallengeUserResponse(GrpcUserInfo User);
public record GrpcUserInfo(long UserId, string UserName);
public record GrpcGetUserByIdRequest(long UserId);
public record GrpcGetUserByIdResponse(GrpcUserInfo User);
