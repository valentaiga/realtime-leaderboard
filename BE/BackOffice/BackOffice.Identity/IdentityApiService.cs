using BackOffice.Identity.Grpc;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace BackOffice.Identity;

public class IdentityApiService : IdentityApi.IdentityApiBase
{
    private ulong _counter = 0;
    public override Task<GrpcLoginResponse> LoginUser(GrpcLoginRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GrpcLoginResponse
        {
            UserId = Interlocked.Increment(ref _counter),
            Username = request.Username,
            JwtToken = "some_jwt.hahaha"
        });
    }

    public override Task<GrpcRefreshUserTokenResponse> RefreshUserToken(GrpcRefreshUserTokenRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GrpcRefreshUserTokenResponse
        {
            JwtToken = "some_jwt.hahaha"
        });
    }

    public override Task<Empty> LogoutUser(GrpcLogoutUserRequest request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }
}