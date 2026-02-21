using System.Security.Claims;
using BackOffice.Identity.Grpc;
using Microsoft.AspNetCore.Mvc;

namespace FrontOffice.Web.Identity;

public static class IdentityController
{
    public static async Task<IResult> Login([FromBody] LoginRequest request, IdentityApi.IdentityApiClient identityClient, CancellationToken ct)
    {
        // todo vm: validate all endpoints

        var req = new GrpcLoginRequest
        {
            Username = request.Username,
            Password = request.Password
        };

        var resp = await identityClient.LoginUserAsync(req, cancellationToken: ct);

        if (resp == null)
            return Results.BadRequest("Invalid credentials");

        return Results.Ok(
            new LoginResponse(
                resp.JwtToken,
                new UserShortInfo(resp.UserId, resp.Username)));
    }

    public static async Task<IResult> RefreshToken([FromBody] RefreshTokenRequest request, IdentityApi.IdentityApiClient identityClient, HttpContext context)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        var req = new GrpcRefreshUserTokenRequest
        {
            UserId = userId,
            RefreshToken = request.RefreshToken
        };
        var resp = await identityClient.RefreshUserTokenAsync(req, cancellationToken: context.RequestAborted);
        if (resp == null)
            return Results.NotFound();

        return Results.Ok(new RefreshTokenResponse(resp.JwtToken));
    }
    
    public static async Task<IResult> Logout(IdentityApi.IdentityApiClient identityClient, HttpContext context)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        await identityClient.LogoutUserAsync(new GrpcLogoutUserRequest { UserId = userId }, cancellationToken: context.RequestAborted);
        return Results.Ok(new LogoutResponse("Logged out successfully"));
    }
}