using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AwesomeAssertions;
using FrontOffice.Web.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tests.Common;
using Tests.Common.FrontOffice.Web;

namespace Tests.UnitTests.FrontOffice.Authentication;

[Collection(TestConstants.TestCollections.FrontOfficeUnitTests)]
public class JwtTokenServiceTests(FrontOfficeUnitTestFixture fixture) : IClassFixture<FrontOfficeUnitTestFixture>
{
    private const long UserId = 1001;
    private const string Username = "username";

    private JwtTokenService JwtTokenService => fixture.Host.GetRequiredService<JwtTokenService>();
    private JwtSecurityTokenHandler JwtSecurityTokenHandler => fixture.Host.GetRequiredService<JwtSecurityTokenHandler>();
    private JwtOptions JwtOptions => fixture.Host.GetRequiredService<IOptions<JwtOptions>>().Value;

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