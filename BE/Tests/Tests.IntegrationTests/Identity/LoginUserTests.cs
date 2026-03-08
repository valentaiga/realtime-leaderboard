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
    }

    [Fact]
    public async Task Login_InvalidPassword_Failure()
    {
        // arrange
        var request = new LoginRequest(_username, "something wrong");

        // act
        var response = await _client.PostAsync("api/identity/login", JsonContent.Create(request));

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
        var response = await _client.PostAsync("api/identity/login", JsonContent.Create(request));

        // assert
        var error = await response.AssertErrorResponseAsync<ApiError>(HttpStatusCode.BadRequest);
        error.Message.Should().Be("User not found or has incorrect password");
    }
}