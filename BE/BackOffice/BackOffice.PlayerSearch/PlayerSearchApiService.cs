using BackOffice.PlayerSearch.Grpc;
using Grpc.Core;

namespace BackOffice.PlayerSearch;

public class AutoCompleteApiService : PlayerSearchApi.PlayerSearchApiBase
{
    public override Task<GrpcPlayerSearchResult> FindPlayersByUsername(FindPlayersByUsernameRequest request, ServerCallContext context)
    {
        return base.FindPlayersByUsername(request, context);
    }
}