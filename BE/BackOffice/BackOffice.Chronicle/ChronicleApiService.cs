using BackOffice.Chronicle.Grpc;
using Grpc.Core;

namespace BackOffice.Chronicle;

public class ChronicleApiService(MatchService matchService) : ChronicleApi.ChronicleApiBase
{
    public override async Task<FilterResult_GrpcMatchInfo> GetPlayerMatches(GetPlayerMatchesFilter request, ServerCallContext context)
    {
        var result = await matchService.GetByFilterAsync(
            request.PlayerId?.FromGrpcFilterDescriptor(),
            request.PlayerWon?.Value,
            request.StartedAt?.FromGrpcFilterDescriptor(),
            request.FinishedAt?.FromGrpcFilterDescriptor(),
            request.Limit,
            request.Offset,
            context.CancellationToken);

        return new FilterResult_GrpcMatchInfo()
        {
            Data = { result.Data.ToGrpc() },
            Total = result.Total
        };
    }
}