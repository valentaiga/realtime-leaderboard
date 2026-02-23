using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AwesomeAssertions;
using FrontOffice.Web.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tests.Common.FrontOffice.Web;

namespace Tests.UnitTests.FrontOffice.Authentication;

public class JwtTokenServiceTests : FrontOfficeTestBase
{
    private const ulong UserId = 1001;
    private const string Username = "username";

    private JwtTokenService JwtTokenService => GetRequiredService<JwtTokenService>();
    private JwtSecurityTokenHandler JwtSecurityTokenHandler => GetRequiredService<JwtSecurityTokenHandler>();
    private JwtOptions JwtOptions => GetRequiredService<IOptions<JwtOptions>>().Value;

    [Fact]
    public async Task GenerateJwtToken_TokenIsValid()
    {
        // arrange
        var tokenValidationParameters = new TokenValidationParameters 
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtOptions.SecretKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = JwtOptions.Audience,
            ValidIssuer = JwtOptions.Issuer,
        };

        // act
        var token = JwtTokenService.GenerateJwtToken(UserId, Username);

        // assert
        var validationResult = await JwtSecurityTokenHandler.ValidateTokenAsync(token, tokenValidationParameters);
        validationResult.IsValid.Should().BeTrue();
        validationResult.ClaimsIdentity.Claims.GetUserId().Should().Be(UserId);
        validationResult.ClaimsIdentity.Claims.GetUsername().Should().Be(Username);

    }

    [Fact]
    public void GenerateJwtToken_TokenHasValidExpirationDate()
    {
        // act
        var token = JwtTokenService.GenerateJwtToken(UserId, Username);

        // assert
        var jwtToken = JwtSecurityTokenHandler.ReadJwtToken(token);
        (jwtToken.ValidTo - jwtToken.ValidFrom).Should().Be(JwtOptions.TokenExpiration);
    }
}