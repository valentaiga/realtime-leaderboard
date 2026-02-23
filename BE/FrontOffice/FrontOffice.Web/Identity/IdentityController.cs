using BackOffice.Identity.Grpc;
using FrontOffice.Web.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace FrontOffice.Web.Identity;

public static class IdentityController
{
    public static async Task<IResult> Login([FromBody] LoginRequest request, IdentityApi.IdentityApiClient identityClient, JwtTokenService jwtTokenService, CancellationToken ct)
    {
        // todo vm: validate all endpoints

        var req = new GrpcChallengeUserRequest
        {
            Username = request.Username,
            Password = request.Password
        };

        var resp = await identityClient.ChallengeUserAsync(req, cancellationToken: ct);
        var jwtToken = jwtTokenService.GenerateJwtToken(resp.UserId, resp.UserName);

        return Results.Ok(
            new LoginResponse(
                jwtToken,
                new UserShortInfo(resp.UserId, resp.UserName)));
    }

    public static async Task<IResult> RefreshToken([FromBody] RefreshTokenRequest request, JwtTokenService jwtTokenService, IdentityApi.IdentityApiClient identityClient, HttpContext context)
    {
        var validationResult = await jwtTokenService.ValidateRefreshTokenAsync(request.JwtToken);

        if (!validationResult.IsValid)
            return Results.BadRequest("Invalid token");

        var userId = validationResult.ClaimsIdentity.Claims.GetUserId();
        var username = validationResult.ClaimsIdentity.Claims.GetUsername();
        var jwtToken = jwtTokenService.GenerateJwtToken(userId, username);

        return Results.Ok(new RefreshTokenResponse(jwtToken));
    }

    // logout should be on client side
    public static Task<IResult> Logout() =>
        Task.FromResult(Results.Ok(new LogoutResponse("Logged out successfully")));
}