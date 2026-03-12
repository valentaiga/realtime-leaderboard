using System.ServiceModel;
using ProtoBuf;

[module: CompatibilityLevel(CompatibilityLevel.Level300)] // configures how Guid, DateTime etc are handled
namespace BackOffice.PlayerSearch.Grpc;

[ServiceContract]
public interface IPlayerSearchApi
{
    [OperationContract]
    Task<GrpcPlayerSearchResult> FindPlayersByUsername(FindPlayersByUsernameRequest request, CancellationToken ct);
}