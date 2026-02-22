namespace BackOffice.Identity.Grpc;

[ServiceContract]
public interface IIdentityApi
{
    [OperationContract]
    Task<GrpcUserInfo> ChallengeUser(GrpcChallengeUserRequest request, CancellationToken ct);

    [OperationContract]
    Task<GrpcUserInfo> GetUserById(GetUserByIdRequest userId, CancellationToken ct);
}

public record GrpcChallengeUserRequest(string Username, string Password);
public record GrpcUserInfo(ulong UserId, string UserName);
public record GetUserByIdRequest(ulong UserId);
