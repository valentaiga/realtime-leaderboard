using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AwesomeAssertions;
using FrontOffice.Web.Authentication;
using FrontOffice.Web.Identity;
using Tests.Common.BackOffice.Identity;
using Tests.Common.FrontOffice.Web;

namespace Tests.UnitTests.FrontOffice;

public class IdentityApiTests : FrontOfficeTestBase
{
    [Fact]
    public async Task Login_Ok()
    {
        // arrange
        const ulong userId = 123;
        await using var identityHost = new IdentityGrpcTestHost(
            grpc => grpc.ChallengeUserResult = req => new () { UserId = userId, UserName = req.Username});
        using var client = CreateClient();
        var loginRequest = new LoginRequest("username", "password");

        // act
        var response = await client.PostAsync("api/identity/login", JsonContent.Create(loginRequest));

        // assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        loginResponse.Should().NotBeNull();
        loginResponse.Token.Should().NotBeNullOrEmpty();
        loginResponse.User.Id.Should().Be(userId);
        loginResponse.User.Username.Should().Be(loginRequest.Username);
    }

    [Fact(Skip = "Currently ChallengeUser call throws an error which considered as RpcError, it could be handled in future.")]
    public async Task Login_BadRequest()
    {
        // arrange
        await using var identityHost = new IdentityGrpcTestHost(
            grpc => grpc.ChallengeUserResult = _ => throw new KeyNotFoundException("User not found or has incorrect password"));
        using var client = CreateClient();
        var loginRequest = new LoginRequest("username", "password");

        // act
        var response = await client.PostAsync("api/identity/login", JsonContent.Create(loginRequest));

        // assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.ReasonPhrase.Should().Be("User not found or has incorrect password");
    }

    [Fact]
    public async Task RefreshToken_Ok()
    {
        // arrange
        const ulong userId = 123;
        const string username = "some username";
        
        await using var identityHost = new IdentityGrpcTestHost(
            grpc => grpc.GetUserByIdResult = req => new () { UserId = userId, UserName = username});
        using var client = CreateClient();
        var token = GetRequiredService<JwtTokenService>().GenerateJwtToken(userId, username);
        var refreshRequest = new RefreshTokenRequest(token);

        // act
        var response = await client.PostAsync("api/identity/refresh", JsonContent.Create(refreshRequest));

        // assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var refreshTokenResponse = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>();
        refreshTokenResponse.Should().NotBeNull();
        refreshTokenResponse.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Logout_Ok()
    {
        // arrange
        const ulong userId = 123;
        const string username = "some username";

        var token = GetRequiredService<JwtTokenService>().GenerateJwtToken(userId, username);
        using var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // act
        var response = await client.PostAsync("api/identity/logout", null);

        // assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task Logout_Unauthorized()
    {
        // arrange
        using var client = CreateClient();

        // act
        var response = await client.PostAsync("api/identity/logout", null);

        // assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}