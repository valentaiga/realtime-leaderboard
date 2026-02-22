using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FrontOffice.Web.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? throw new KeyNotFoundException("Jwt secret key not configured"));

        services.AddOptions<JwtOptions>()
            .Configure(options => jwtSettings.Bind(options));
        services
            .AddSingleton<JwtTokenService>()
            .AddSingleton<JwtSecurityTokenHandler>()
            .AddAuthorization();
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            });

        return services;
    }
    
    public static ulong GetUserId(this ClaimsPrincipal principal) => principal.Claims.GetUserId();

    public static ulong GetUserId(this IEnumerable<Claim> claims) => ulong.Parse(claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

    public static string GetUsername(this IEnumerable<Claim> claims) => claims.First(x => x.Type == ClaimTypes.Name).Value;
}