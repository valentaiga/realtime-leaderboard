using BackOffice.Chronicle.Grpc;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace BackOffice.Chronicle;

public class ChronicleApiService : ChronicleApi.ChronicleApiBase
{
    public override async Task<FilterResult_MatchInfo> GetPlayerMatches(GetPlayerMatchesFilter request, ServerCallContext context)
    {
        var matches = new MatchInfo[]
        {
            new()
            {
                Losers = { 1, 2, 3, 4, 5 },
                Winners = { 6, 7, 8, 9, 10 },
                StartedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                FinishedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                MatchId = Guid.NewGuid().ToString()
            }
        };
        var total = 1000ul;
        
        return new FilterResult_MatchInfo()
        {
            Data = { matches },
            Total = total
        };
    }
}