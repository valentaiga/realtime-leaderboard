using BackOffice.Chronicle.Grpc;
using Common.Filtering;
using Microsoft.AspNetCore.Mvc;

namespace FrontOffice.Web.Api.Player;

public static class PlayerController
{
    public static async Task<IResult> GetPlayerHistory([FromBody] MatchFilterRequest request, ChronicleApi.ChronicleApiClient chronicleApiClient, CancellationToken ct)
    {
        var spec = MatchFilterRequestFilterSpecification.Map(request);
        var req = new GetPlayerMatchesFilter
        {
            PlayerId = spec.PlayerIdDescriptor?.ToGrpcFilterDescriptor(),
            FinishedAt = spec.FinishedAtDescriptor?.ToGrpcFilterDescriptor(),
            StartedAt = spec.StartedAtDescriptor?.ToGrpcFilterDescriptor(),
        };
        var result = await chronicleApiClient.GetPlayerMatchesAsync(req, cancellationToken: ct);
        var response = new FilterResult<Match>
        {
            Total = result.Total,
            Data = result.Data.FromRepeatedField()
        };
        return Results.Ok(response);
    }
}