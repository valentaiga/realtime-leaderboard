using System.ServiceModel;

namespace BackOffice.Identity.Grpc;

[ServiceContract]
public interface IIdentityApi
{
    [OperationContract]
    Task<GrpcChallengeUserResponse> ChallengeUser(GrpcChallengeUserRequest request, CancellationToken ct);

    [OperationContract]
    Task<GrpcGetUserByIdResponse> GetUserById(GrpcGetUserByIdRequest userId, CancellationToken ct);

    [OperationContract]
    Task RegisterUser(GrpcRegisterUserRequest request, CancellationToken ct);
}