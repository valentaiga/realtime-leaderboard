using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FrontOffice.Web.Authentication;

public class JwtTokenService
{
    private readonly SigningCredentials _signingCredentials;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly JwtSecurityTokenHandler _jwtTokenHandler;
    private readonly TokenValidationParameters _refreshTokenValidationParameters;

    public JwtTokenService(IOptions<JwtOptions> jwtOptions, JwtSecurityTokenHandler jwtTokenHandler)
    {
        _jwtOptions = jwtOptions;
        _jwtTokenHandler = jwtTokenHandler;

        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Value.SecretKey)),
            SecurityAlgorithms.HmacSha256Signature);
        _refreshTokenValidationParameters = new TokenValidationParameters 
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingCredentials.Key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = _jwtOptions.Value.RefreshTokenExpiration
        };
    }

    public string GenerateJwtToken(ulong userId, string username)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(),
            Expires = DateTime.UtcNow + _jwtOptions.Value.TokenExpiration,
            Issuer = _jwtOptions.Value.Issuer,
            Audience = _jwtOptions.Value.Audience,
            SigningCredentials = _signingCredentials
        };
        tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
        tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Name, username));

        var token = _jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        return _jwtTokenHandler.WriteToken(token);
    }

    public Task<TokenValidationResult> ValidateRefreshTokenAsync(string token) =>
        _jwtTokenHandler.ValidateTokenAsync(token, _refreshTokenValidationParameters);
}