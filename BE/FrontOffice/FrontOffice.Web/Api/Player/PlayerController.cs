using BackOffice.PlayerSearch.Grpc;
using Microsoft.AspNetCore.Mvc;

namespace FrontOffice.Web.Api.Player;

public static class PlayerController
{
    public static async Task<IResult> SearchPlayersByUsername([FromQuery] string username, PlayerSearchApi.PlayerSearchApiClient autoCompleteApiClient, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(username))
            return Results.BadRequest(new ApiError("Username is empty"));

        var request = new FindPlayersByUsernameRequest
        {
            Username = username
        };
        var result = await autoCompleteApiClient.FindPlayersByUsernameAsync(request, cancellationToken: ct);

        return Results.Ok(new SearchPlayersResponse
        {
            Players = result.Top.FromGrpc()
        });
    }
}