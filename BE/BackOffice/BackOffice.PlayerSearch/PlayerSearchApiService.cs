using BackOffice.PlayerSearch.DynamoDb;
using BackOffice.PlayerSearch.Grpc;
using Grpc.Core;

namespace BackOffice.PlayerSearch;

public class PlayerSearchApiService(PlayerSearchService playerSearchService) : PlayerSearchApi.PlayerSearchApiBase
{
    public override async Task<GrpcPlayerSearchResult> FindPlayersByUsername(FindPlayersByUsernameRequest request, ServerCallContext context)
    {
        var result = await playerSearchService.FindPlayersAsync(request.Username, ct: context.CancellationToken);
        return new GrpcPlayerSearchResult
        {
            Top = { result.Select(x => new GrpcPlayer { PlayerId = x.Id, Username = x.Username }) }
        };
    }
}