using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BackOffice.Identity.Identity;

public class JwtTokenService(IOptions<JwtOptions> options, JwtSecurityTokenHandler jwtTokenHandler)
{
    private readonly SigningCredentials _signingCredentials = new(
        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(options.Value.SecretKey)),
        SecurityAlgorithms.HmacSha256Signature);

    public string GenerateJwtToken(ulong userId, string username)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(),
            Expires = DateTime.UtcNow.AddHours(options.Value.ExpirationHours),
            Issuer = options.Value.Issuer,
            Audience = options.Value.Audience,
            SigningCredentials = _signingCredentials
        };
        tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
        tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Name, username));

        var token = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        return jwtTokenHandler.WriteToken(token);
    }
}