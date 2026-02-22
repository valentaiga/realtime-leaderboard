using BackOffice.Identity.Grpc;
using BackOffice.Identity.Identity;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace BackOffice.Identity;

public class IdentityApiService(UserService userService, JwtTokenService jwtTokenService) : IdentityApi.IdentityApiBase
{
    public override async Task<GrpcLoginResponse> LoginUser(GrpcLoginRequest request, ServerCallContext context)
    {
        var loginResult = await userService.LoginUserAsync(request.Username, request.Password, context.CancellationToken);
        var jwtToken = jwtTokenService.GenerateJwtToken(loginResult.UserId, request.Username);
        return new GrpcLoginResponse
        {
            UserId = loginResult.UserId,
            Username = request.Username,
            JwtToken = jwtToken
        };
    }

    public override async Task<GrpcRefreshUserTokenResponse> RefreshUserToken(GrpcRefreshUserTokenRequest request, ServerCallContext context)
    {
        var username = await userService.GetUserByIdAsync(request.UserId, context.CancellationToken);
        var jwtToken = jwtTokenService.GenerateJwtToken(request.UserId, username);
        return new GrpcRefreshUserTokenResponse
        {
            JwtToken = jwtToken
        };
    }

    public override Task<Empty> LogoutUser(GrpcLogoutUserRequest request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }
}