using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AwesomeAssertions;
using Common.Primitives;
using FrontOffice.Web.Api;
using FrontOffice.Web.Api.Identity;
using FrontOffice.Web.Authentication;
using Grpc.Core;
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
            grpc => grpc.ChallengeUserResult = req => new() { User = new() { UserId = userId, UserName = req.Username } });
        using var client = CreateClient();
        var loginRequest = new LoginRequest("username", "password");

        // act
        var response = await client.PostAsync("api/identity/login", JsonContent.Create(loginRequest));

        // assert
        var loginResponse = await AssertSuccessResponseAsync<LoginResponse>(response);
        loginResponse.Token.Should().NotBeNullOrEmpty();
        loginResponse.User.Id.Should().Be(userId);
        loginResponse.User.Username.Should().Be(loginRequest.Username);
    }

    [Fact]
    public async Task Login_BadRequest()
    {
        // arrange
        await using var identityHost = new IdentityGrpcTestHost(
            grpc => grpc.ChallengeUserResult = _ => throw new BusinessException("User not found or has incorrect password", (int)StatusCode.NotFound));
        using var client = CreateClient();
        var loginRequest = new LoginRequest("username", "password");

        // act
        var response = await client.PostAsync("api/identity/login", JsonContent.Create(loginRequest));

        // assert
        var error = await AssertErrorResponseAsync<ApiError>(response, HttpStatusCode.BadRequest);
        error.Message.Should().Be("User not found or has incorrect password");
    }

    [Fact]
    public async Task RefreshToken_Ok()
    {
        // arrange
        const ulong userId = 123;
        const string username = "some username";
        
        await using var identityHost = new IdentityGrpcTestHost(
            grpc => grpc.GetUserByIdResult = req => new() { User = new() { UserId = req.UserId, UserName = username } });
        using var client = CreateClient();
        var token = GetRequiredService<JwtTokenService>().GenerateJwtToken(userId, username);
        var refreshRequest = new RefreshTokenRequest(token);

        // act
        var response = await client.PostAsync("api/identity/refresh", JsonContent.Create(refreshRequest));

        // assert
        var refreshTokenResponse = await AssertSuccessResponseAsync<RefreshTokenResponse>(response);
        refreshTokenResponse.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshToken_InvalidToken()
    {
        // arrange
        using var client = CreateClient();
        const string invalidToken = "doasjkmfopsajptgoj3wqwapotpoa2.dawspdfhawoihfgoawifhoiawfoi23ro13n9f8";
        var refreshRequest = new RefreshTokenRequest(invalidToken);

        // act
        var response = await client.PostAsync("api/identity/refresh", JsonContent.Create(refreshRequest));

        // assert
        var error = await AssertErrorResponseAsync<ApiError>(response, HttpStatusCode.BadRequest);
        error.Message.Should().Be("Invalid token");
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
        AssertSuccessResponse(response);
    }

    [Fact]
    public async Task Logout_InvalidToken_Unauthorized()
    {
        // arrange
        using var client = CreateClient();
        const string invalidToken = "doasjkmfopsajptgoj3wqwapotpoa2.dawspdfhawoihfgoawifhoiawfoi23ro13n9f8";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalidToken);

        // act
        var response = await client.PostAsync("api/identity/logout", null);

        // assert
        AssertErrorResponse(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_NoToken_Unauthorized()
    {
        // arrange
        using var client = CreateClient();

        // act
        var response = await client.PostAsync("api/identity/logout", null);

        // assert
        AssertErrorResponse(response, HttpStatusCode.Unauthorized);
    }
}