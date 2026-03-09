using System.Net.Http.Headers;
using FrontOffice.Web.Api.Identity;

namespace Tests.IntegrationTests.Identity;

public class LoginUserFixture : IDisposable
{
    public RegisterRequest TestUser { get; } = new (1, "test username", "test password");

    private bool _isSeeded;

    public async Task SeedUsers(HttpClient webClient)
    {
        if (_isSeeded)
            return;

        await webClient.PostAsync("api/identity/register", JsonContent.Create(TestUser));
        _isSeeded = true;
    }
    
    public void Dispose()
    {
        IntegrationTestFixture.CleanIdentityDb();
        GC.SuppressFinalize(this);
    }
}

[Collection(TestConstants.TestCollections.IntegrationTests)]
public class LoginUserTests : IntegrationTestBase, IClassFixture<LoginUserFixture>
{
    private readonly string _username;
    private readonly HttpClient _client;
    private readonly long _userId;
    private readonly string _password;

    public LoginUserTests(IntegrationTestFixture fixture, LoginUserFixture localFixture) : base(fixture)
    {
        _username = localFixture.TestUser.Username;
        _userId = (long)localFixture.TestUser.Id;
        _password = localFixture.TestUser.Password;
        _client = fixture.Web.CreateClient();
        localFixture.SeedUsers(_client).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Login_Success()
    {
        // arrange
        var request = new LoginRequest(_username, _password);

        // act
        var response = await _client.PostAsync("api/identity/login", JsonContent.Create(request));

        // assert
        var loginResponse = await response.AssertSuccessResponseAsync<LoginResponse>();
        loginResponse.Token.Should().NotBeNull();
        loginResponse.User.Id.Should().Be(_userId);
        loginResponse.User.Username.Should().Be(_username);
        await AssertValidAuthorizationAsync(loginResponse.Token);
    }

    [Fact]
    public async Task Login_InvalidPassword_Failure()
    {
        // arrange
        var request = new LoginRequest(_username, "something wrong");

        // act
        using var response = await _client.PostAsync("api/identity/login", JsonContent.Create(request));

        // assert
        var error = await response.AssertErrorResponseAsync<ApiError>(HttpStatusCode.BadRequest);
        error.Message.Should().Be("User not found or has incorrect password");
    }

    [Fact]
    public async Task Login_InvalidLogin_Failure()
    {
        // arrange
        var request = new LoginRequest("some username", _password);

        // act
        using var response = await _client.PostAsync("api/identity/login", JsonContent.Create(request));

        // assert
        var error = await response.AssertErrorResponseAsync<ApiError>(HttpStatusCode.BadRequest);
        error.Message.Should().Be("User not found or has incorrect password");
    }

    [Fact]
    public async Task Logout_InvalidToken_Failure()
    {
        // arrange
        using var client = Fixture.Web.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "as4sDgtf32.asfasawfsome_invalid_token");

        // act
        using var response = await client.PostAsync("api/identity/logout", null);

        // assert
        response.AssertErrorResponse(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_Success()
    {
        // arrange
        var loginRequest = new LoginRequest(_username, _password);
        using var loginResponse = await _client.PostAsync("api/identity/login", JsonContent.Create(loginRequest));
        var loginResp = await loginResponse.AssertSuccessResponseAsync<LoginResponse>();
        var request = new RefreshTokenRequest(loginResp.Token);

        // act
        using var response = await _client.PostAsync("api/identity/refresh", JsonContent.Create(request));

        // assert
        var result = await response.AssertSuccessResponseAsync<RefreshTokenResponse>();
        result.Token.Should().NotBeNull();
        await AssertValidAuthorizationAsync(result.Token);
    }

    [Fact]
    public async Task RefreshToken_NoToken_Failure()
    {
        // act
        using var response = await _client.PostAsync("api/identity/refresh", null);

        // assert
        var error = await response.AssertErrorResponseAsync<ApiError>(HttpStatusCode.BadRequest);
        error.Message.Should().Be("Request parameters are invalid");
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_Failure()
    {
        // arrange
        var request = new RefreshTokenRequest("as4sDgtf32.asfasawfsome_invalid_token");

        // act
        using var response = await _client.PostAsync("api/identity/refresh", JsonContent.Create(request));

        // assert
        var error = await response.AssertErrorResponseAsync<ApiError>(HttpStatusCode.BadRequest);
        error.Message.Should().Be("Invalid token");
    }

    private async Task AssertValidAuthorizationAsync(string token)
    {
        using var client = Fixture.Web.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = await client.PostAsync("api/identity/logout", null);
        await response.AssertSuccessResponseAsync<LogoutResponse>();
    }
}