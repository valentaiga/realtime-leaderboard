using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BackOffice.Identity.Grpc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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

        return Results.Ok(
            new LoginResponse(
                resp.JwtToken,
                new UserShortInfo(resp.UserId, resp.Username)));
    }

    public static async Task<IResult> RefreshToken([FromBody] RefreshTokenRequest request, JwtSecurityTokenHandler tokenHandler, IOptionsMonitor<JwtBearerOptions> jwtOptionsMonitor, IdentityApi.IdentityApiClient identityClient, HttpContext context)
    {
        var options = jwtOptionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);
        var validationResult = await tokenHandler.ValidateTokenAsync(request.JwtToken, new TokenValidationParameters // todo vm: reduce allocation with DI 
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = options.TokenValidationParameters.IssuerSigningKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.FromDays(7)
        });

        if (!validationResult.IsValid)
            return Results.BadRequest("Invalid token");

        var req = new GrpcRefreshUserTokenRequest
        {
            UserId = validationResult.ClaimsIdentity.Claims.GetUserId()
        };
        var resp = await identityClient.RefreshUserTokenAsync(req, cancellationToken: context.RequestAborted);

        return Results.Ok(new RefreshTokenResponse(resp.JwtToken));
    }
    
    public static async Task<IResult> Logout(IdentityApi.IdentityApiClient identityClient, HttpContext context)
    {
        var userId = context.User.GetUserId();
        await identityClient.LogoutUserAsync(new GrpcLogoutUserRequest { UserId = userId }, cancellationToken: context.RequestAborted);
        return Results.Ok(new LogoutResponse("Logged out successfully"));
    }

    private static ulong GetUserId(this ClaimsPrincipal principal) => principal.Claims.GetUserId();
    private static ulong GetUserId(this IEnumerable<Claim> claims) => ulong.Parse(claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
}