using BackOffice.Chronicle.Grpc;
using Grpc.Core;

namespace BackOffice.Chronicle;

public class ChronicleApiService(MatchService matchService) : ChronicleApi.ChronicleApiBase
{
    public override async Task<FilterResult_MatchInfo> GetPlayerMatches(GetPlayerMatchesFilter request, ServerCallContext context)
    {
        var result = await matchService.GetByFilterAsync(
            request.PlayerId?.FromGrpcFilterDescriptor(),
            request.StartedAt?.FromGrpcFilterDescriptor(),
            request.FinishedAt?.FromGrpcFilterDescriptor(),
            request.Limit,
            request.Offset,
            context.CancellationToken);
        
        return new FilterResult_MatchInfo
        {
            Data = { result.Data.ToGrpc() },
            Total = result.Total
        };
    }
}