using BackOffice.Identity.Grpc;
using Grpc.Core;

namespace Tests.Common.BackOffice.Identity;

public class IdentityGrpcTestHost(Action<IdentityApiMockBehaviour>? configureBehaviour = null) : GrpcTestHost<IdentityApiMock, IdentityApiMockBehaviour>(5010, configureBehaviour);

public class IdentityApiMock(IdentityApiMockBehaviour behaviour) : IdentityApi.IdentityApiBase
{
    public override async Task<GrpcChallengeUserResponse> ChallengeUser(GrpcChallengeUserRequest request, ServerCallContext context)
    {
        return behaviour.ChallengeUserResult.Invoke(request);
    }

    public override async Task<GrpcGetUserByIdResponse> GetUserById(GrpcGetUserByIdRequest request, ServerCallContext context)
    {
        return behaviour.GetUserByIdResult.Invoke(request);
    }
}

public class IdentityApiMockBehaviour
{
    public Func<GrpcChallengeUserRequest, GrpcChallengeUserResponse> ChallengeUserResult { get; set; } =
        _ => throw new NotImplementedException($"{nameof(IdentityApiMockBehaviour)}.{nameof(ChallengeUserResult)} not implemented");

    public Func<GrpcGetUserByIdRequest, GrpcGetUserByIdResponse> GetUserByIdResult { get; set; } =
        _ => throw new NotImplementedException($"{nameof(IdentityApiMockBehaviour)}.{nameof(GetUserByIdResult)} not implemented");
}