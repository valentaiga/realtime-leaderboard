namespace BackOffice.Identity.Grpc;

[ServiceContract]
public interface IIdentityApi
{
    [OperationContract]
    Task<GrpcChallengeUserResponse> ChallengeUser(GrpcChallengeUserRequest request, CancellationToken ct);

    [OperationContract]
    Task<GrpcGetUserByIdResponse> GetUserById(GrpcGetUserByIdRequest userId, CancellationToken ct);
}

public record GrpcChallengeUserRequest(string Username, string Password);
public record GrpcChallengeUserResponse(GrpcUserInfo User);
public record GrpcUserInfo(ulong UserId, string UserName);
public record GrpcGetUserByIdRequest(ulong UserId);
public record GrpcGetUserByIdResponse(GrpcUserInfo User);
