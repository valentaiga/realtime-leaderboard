using BackOffice.Identity.Grpc;
using BackOffice.Identity.Identity;
using Grpc.Core;

namespace BackOffice.Identity;

public class IdentityApiService(UserService userService) : IdentityApi.IdentityApiBase
{
    public override async Task<GrpcChallengeUserResponse> ChallengeUser(GrpcChallengeUserRequest request, ServerCallContext context)
    {
        var loginResult = await userService.LoginUserAsync(request.Username, request.Password, context.CancellationToken);
        return new()
        {
            User = new()
            {
                UserId = loginResult.UserId,
                UserName = request.Username,
            }
        };
    }

    public override async Task<GrpcGetUserByIdResponse> GetUserById(GrpcGetUserByIdRequest request, ServerCallContext context)
    {
        var userName = await userService.GetUserByIdAsync(request.UserId, context.CancellationToken);
        return new()
        {
            User = new()
            {
                UserId = request.UserId,
                UserName = userName
            }
        };
    }
}