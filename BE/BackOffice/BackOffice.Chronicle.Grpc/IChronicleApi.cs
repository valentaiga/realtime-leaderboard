using System.ServiceModel;
using Common.Filtering;
using ProtoBuf;

[module: CompatibilityLevel(CompatibilityLevel.Level300)] // configures how Guid, DateTime etc are handled
namespace BackOffice.Chronicle.Grpc;

[ServiceContract]
public interface IChronicleApi
{
    [OperationContract]
    Task<FilterResult<GrpcMatchInfo>> GetPlayerMatches(GetPlayerMatchesFilter request, CancellationToken ct);
}