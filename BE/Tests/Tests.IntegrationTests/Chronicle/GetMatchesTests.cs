using System.Text.Json;
using BackOffice.Chronicle.Data.Models;
using BackOffice.Chronicle.Database;
using Common.Filtering;
using FrontOffice.Web.Api.Matches;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.IntegrationTests.Chronicle;

public class GetMatchesFixture : IDisposable
{
    public long[] Winners { get; } = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
    public long[] Losers { get; } = [11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23];
    public List<MatchDto> Matches { get; } = [];

    private bool _isSeeded;

    public GetMatchesFixture()
    {
        AddMatches(40, new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        AddMatches(40, new DateTime(2000, 1, 2, 12, 0, 0, DateTimeKind.Utc));
        AddMatches(40, new DateTime(2000, 1, 3, 12, 0, 0, DateTimeKind.Utc));
        AddMatches(40, new DateTime(2000, 1, 4, 12, 0, 0, DateTimeKind.Utc));
        AddMatches(40, new DateTime(2000, 1, 5, 12, 0, 0, DateTimeKind.Utc));
        AddMatches(40, new DateTime(2000, 1, 6, 12, 0, 0, DateTimeKind.Utc));
        AddMatches(40, new DateTime(2000, 1, 7, 12, 0, 0, DateTimeKind.Utc));
        AddMatches(40, new DateTime(2000, 1, 8, 12, 0, 0, DateTimeKind.Utc));
        return;

        void AddMatches(int count, DateTime startedAt)
        {
            for (var i = 0; i < count; i++)
            {
                var matchDto = new MatchDto
                {
                    MatchId = $"{DateTime.UtcNow.Ticks:D}{Guid.NewGuid():N}",
                    StartedAt = startedAt,
                    FinishedAt = startedAt.AddHours(1),
                    Players = []
                };
                Random.Shared.Shuffle(Winners);
                Random.Shared.Shuffle(Losers);
                foreach (var playerId in Winners[..5])
                    matchDto.Players.Add(new MatchPlayerDto { IsWin = true, PlayerId = playerId });
                foreach (var playerId in Losers[..5])
                    matchDto.Players.Add(new MatchPlayerDto { IsWin = false, PlayerId = playerId });
                Matches.Add(matchDto);
            }
        }
    }

    public async Task SeedDbAsync(IMatchRepository repository)
    {
        if (_isSeeded)
            return;

        await Task.WhenAll(
            Matches.Select(x => repository.AddAsync(x, CancellationToken.None)));
        _isSeeded = true;
    }

    public void Dispose()
    {
        IntegrationTestFixture.CleanChronicleDb();
        GC.SuppressFinalize(this);
    }
}

[Collection(TestConstants.TestCollections.IntegrationTests)]
public class GetMatchesTests : IntegrationTestBase, IClassFixture<GetMatchesFixture>
{
    private readonly GetMatchesFixture _localTestsFixture;
    private readonly HttpClient _client;

    public GetMatchesTests(IntegrationTestFixture fixture, GetMatchesFixture localTestsFixture) : base(fixture)
    {
        _localTestsFixture = localTestsFixture;
        _client = fixture.Web.CreateClient();

        var repository = Fixture.Chronicle.Services.GetRequiredService<IMatchRepository>();
        localTestsFixture.SeedDbAsync(repository).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task GetMatches_ByPlayerId_Success()
    {
        // arrange
        var playerId = _localTestsFixture.Winners[3];
        var request = new GetMatchesRequest
        {
            PlayerId = new FilterDescriptor<long>(playerId, FilterOperator.Equals)
        };
        var requestedMatches = _localTestsFixture.Matches.FindAll(x => x.Players.Any(mp => mp.PlayerId == playerId));

        // act
        var response = await _client.PostAsync("/api/matches", JsonContent.Create(request));

        // assert
        var getMatchesResponse = await response.AssertSuccessResponseAsync<FilterResult<Match>>();
        getMatchesResponse.Total.Should().Be(requestedMatches.Count);
        getMatchesResponse.Data.Should().HaveCount(Math.Min((int)request.Limit, requestedMatches.Count));
        getMatchesResponse.Data.Should().AllSatisfy(x => x.Players.Should().Contain(p => p.PlayerId == playerId));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetMatches_ByPlayerId_WithPlayerWon_Success(bool isWin)
    {
        // arrange
        var playerId = _localTestsFixture.Winners[3];
        var request = new GetMatchesRequest
        {
            PlayerId = new FilterDescriptor<long>(playerId, FilterOperator.Equals),
            PlayerWon = isWin
        };
        var requestedMatches = _localTestsFixture.Matches.FindAll(x => x.Players.Any(mp => mp.PlayerId == playerId && mp.IsWin == isWin));
        var str = JsonSerializer.Serialize(request);

        // act
        var response = await _client.PostAsync("/api/matches", JsonContent.Create(request));

        // assert
        var getMatchesResponse = await response.AssertSuccessResponseAsync<FilterResult<Match>>();
        getMatchesResponse.Total.Should().Be(requestedMatches.Count);
        getMatchesResponse.Data.Should().HaveCount(Math.Min((int)request.Limit, requestedMatches.Count));
        getMatchesResponse.Data.Should().AllSatisfy(x => x.Players.Should().Contain(p => p.PlayerId == playerId && p.IsWin == isWin));
    }

    [Theory]
    [InlineData(FilterOperator.GreaterThan)]
    [InlineData(FilterOperator.GreaterThanOrEqual)]
    [InlineData(FilterOperator.LessThan)]
    [InlineData(FilterOperator.LessThanOrEqual)]
    public async Task GetMatches_ByStartedAt_Success(FilterOperator filterOperator)
    {
        // arrange
        var request = new GetMatchesRequest
        {
            StartedAt = new FilterDescriptor<DateTime>(new DateTime(2000, 1, 4, 0, 0, 0, DateTimeKind.Utc), filterOperator)
        };
        var requestedMatches = filterOperator switch
        {
            FilterOperator.GreaterThan => _localTestsFixture.Matches.FindAll(x => x.StartedAt > request.StartedAt.Value),
            FilterOperator.GreaterThanOrEqual => _localTestsFixture.Matches.FindAll(x => x.StartedAt >= request.StartedAt.Value),
            FilterOperator.LessThan => _localTestsFixture.Matches.FindAll(x => x.StartedAt < request.StartedAt.Value),
            FilterOperator.LessThanOrEqual => _localTestsFixture.Matches.FindAll(x => x.StartedAt <= request.StartedAt.Value),
            _ => throw new InvalidOperationException("Wrong test InlineData")
        };

        // act
        var response = await _client.PostAsync("/api/matches", JsonContent.Create(request));

        // assert
        var getMatchesResponse = await response.AssertSuccessResponseAsync<FilterResult<Match>>();
        getMatchesResponse.Total.Should().Be(requestedMatches.Count);
        getMatchesResponse.Data.Should().HaveCount(Math.Min((int)request.Limit, requestedMatches.Count));
    }

    [Theory]
    [InlineData(FilterOperator.GreaterThan)]
    [InlineData(FilterOperator.GreaterThanOrEqual)]
    [InlineData(FilterOperator.LessThan)]
    [InlineData(FilterOperator.LessThanOrEqual)]
    public async Task GetMatches_ByFinishedAt_Success(FilterOperator filterOperator)
    {
        // arrange
        var request = new GetMatchesRequest
        {
            FinishedAt = new FilterDescriptor<DateTime>(new DateTime(2000, 1, 4, 0, 0, 0, DateTimeKind.Utc), filterOperator)
        };
        var requestedMatches = filterOperator switch
        {
            FilterOperator.GreaterThan => _localTestsFixture.Matches.FindAll(x => x.StartedAt > request.FinishedAt.Value),
            FilterOperator.GreaterThanOrEqual => _localTestsFixture.Matches.FindAll(x => x.StartedAt >= request.FinishedAt.Value),
            FilterOperator.LessThan => _localTestsFixture.Matches.FindAll(x => x.StartedAt < request.FinishedAt.Value),
            FilterOperator.LessThanOrEqual => _localTestsFixture.Matches.FindAll(x => x.StartedAt <= request.FinishedAt.Value),
            _ => throw new InvalidOperationException("Wrong test InlineData")
        };

        // act
        var response = await _client.PostAsync("/api/matches", JsonContent.Create(request));

        // assert
        var getMatchesResponse = await response.AssertSuccessResponseAsync<FilterResult<Match>>();
        getMatchesResponse.Total.Should().Be(requestedMatches.Count);
        getMatchesResponse.Data.Should().HaveCount(Math.Min((int)request.Limit, requestedMatches.Count));
    }
}