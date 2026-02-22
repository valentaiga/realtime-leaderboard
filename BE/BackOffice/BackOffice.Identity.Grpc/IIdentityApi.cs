namespace BackOffice.Identity.Grpc;

[ServiceContract]
public interface IIdentityApi
{
    [OperationContract]
    Task<GrpcLoginResponse> LoginUser(GrpcLoginRequest request, CancellationToken ct);

    [OperationContract]
    Task<GrpcRefreshUserTokenResponse> RefreshUserToken(GrpcRefreshUserTokenRequest request, CancellationToken ct);

    [OperationContract]
    Task LogoutUser(GrpcLogoutUserRequest request, CancellationToken ct);
}

public record GrpcLoginRequest(string Username, string Password);
public record GrpcLoginResponse(ulong UserId, string Username, string JwtToken);
public record GrpcRefreshUserTokenRequest(ulong UserId);
public record GrpcRefreshUserTokenResponse(string JwtToken);
public record GrpcLogoutUserRequest(ulong UserId);
