using FrontOffice.Web.Api.Identity;

namespace Tests.IntegrationTests.Identity;

[Collection(TestConstants.TestCollections.IntegrationTests)]
public class RegisterUserTests : IntegrationTestBase
{
    private readonly HttpClient _client;

    public RegisterUserTests(IntegrationTestFixture fixture) : base(fixture)
    {
        _client = fixture.Web.CreateClient();
        IntegrationTestFixture.CleanIdentityDb();
    }

    [Fact]
    public async Task RegisterUser_Success()
    {
        // arrange
        var request = new RegisterRequest(1, "username", "password$1");

        // act
        using var response = await _client.PostAsync("/api/identity/register", JsonContent.Create(request));

        // assert
        response.AssertSuccessResponse();
        await AssertSuccessLoginAsync(request.Username, request.Password);
    }

    [Fact]
    public async Task RegisterUser_Duplicate_Failure()
    {
        // arrange
        var request = new RegisterRequest(1, "username", "password$1");
        using var _ = await _client.PostAsync("/api/identity/register", JsonContent.Create(request));

        // act
        using var response = await _client.PostAsync("/api/identity/register", JsonContent.Create(request));

        // assert
        var error = await response.AssertErrorResponseAsync<ApiError>(HttpStatusCode.BadRequest);
        error.Message.Should().Be("User with same id or username already exists");
    }

    private async Task AssertSuccessLoginAsync(string username, string password)
    {
        using var response = await _client.PostAsync("api/identity/login", JsonContent.Create(new LoginRequest(username, password)));
        response.AssertSuccessResponse();
    }
}