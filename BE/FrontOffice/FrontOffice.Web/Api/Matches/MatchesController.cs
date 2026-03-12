using BackOffice.Chronicle.Grpc;
using Common.Filtering;
using Microsoft.AspNetCore.Mvc;

namespace FrontOffice.Web.Api.Matches;

public static class MatchesController
{
    public static async Task<IResult> GetMatches([FromBody] GetMatchesRequest request, ChronicleApi.ChronicleApiClient chronicleApiClient, CancellationToken ct)
    {
        var req = new GetPlayerMatchesFilter
        {
            PlayerId = request.PlayerId?.ToGrpcFilterDescriptor(),
            FinishedAt = request.FinishedAt?.ToGrpcFilterDescriptor(),
            StartedAt = request.StartedAt?.ToGrpcFilterDescriptor(),
            PlayerWon = request.PlayerWon?.ToGrpcFilterDescriptor(),
            Limit = request.Limit,
            Offset = request.Offset,
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