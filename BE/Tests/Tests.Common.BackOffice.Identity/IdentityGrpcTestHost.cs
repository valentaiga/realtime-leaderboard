using BackOffice.Identity.Grpc;
using Grpc.Core;

namespace Tests.Common.BackOffice.Identity;

public class IdentityGrpcTestHost(Action<IdentityApiMockBehaviour>? configureBehaviour = null) : GrpcTestHost<IdentityApiMock, IdentityApiMockBehaviour>(5010, configureBehaviour);

public class IdentityApiMock(IdentityApiMockBehaviour behaviour) : IdentityApi.IdentityApiBase
{
    
    public override Task<GrpcUserInfo> ChallengeUser(GrpcChallengeUserRequest request, ServerCallContext context) =>
        behaviour.ChallengeUserResult is null
            ? throw new Exception($"{nameof(behaviour.ChallengeUserResult)} not set for a test")
            : Task.FromResult(behaviour.ChallengeUserResult.Invoke(request));

    public override Task<GrpcUserInfo> GetUserById(GetUserByIdRequest request, ServerCallContext context) =>
        behaviour.GetUserByIdResult is null
            ? throw new Exception($"{nameof(behaviour.GetUserByIdResult)} not set for a test")
            : Task.FromResult(behaviour.GetUserByIdResult.Invoke(request));
}

public class IdentityApiMockBehaviour
{
    public Func<GrpcChallengeUserRequest, GrpcUserInfo>? ChallengeUserResult { get; set; }
    public Func<GetUserByIdRequest, GrpcUserInfo>? GetUserByIdResult { get; set; }
}