using BackOffice.Chronicle.Migrations;
using BackOffice.MQ.Messages.MatchStatus;
using Common.MQ.Kafka.Producer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.IntegrationTests.Chronicle;

[Collection(TestConstants.TestCollections.IntegrationTests)]
public class ChronicleKafkaTests : IntegrationTestBase
{
    public ChronicleKafkaTests(IntegrationTestFixture fixture) : base(fixture)
    {
        IntegrationTestFixture.CleanChronicleDb();
    }

    [Fact]
    public async Task MatchStatus_MatchStartedEvent_MatchSkipped()
    {
        // arrange
        var producer = Fixture.Mm.Services.GetRequiredService<IKafkaProducer<string, MatchStatusMessage>>();
        var message = new MatchStatusMessage
        {
            MatchId = $"{DateTime.UtcNow.Ticks:D}{Guid.NewGuid():N}",
            MatchStartedEvent = new([1,2,3,4,5], [6,7,8,9,10], DateTime.UtcNow)
        };
        var offset = Fixture.Chronicle.MatchStatusConsumer.CommitedOffset;

        // act
        await producer.ProduceAsync(message.MatchId, message, CancellationToken.None);

        // assert
        SpinWait.SpinUntil(() => Fixture.Chronicle.MatchStatusConsumer.CommitedOffset == offset + 1, 1_000);
        await using var scope = Fixture.ChronicleMigrations.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ChronicleDbContext>();
        var entities = await dbContext.Matches
            .Include(x => x.Players)
            .ToListAsync();
        entities.Should().BeEmpty();
    }

    [Fact]
    public async Task MatchStatus_MatchFinishedEvent_MatchCreatedInDb()
    {
        // arrange
        var producer = Fixture.Mm.Services.GetRequiredService<IKafkaProducer<string, MatchStatusMessage>>();
        var message = new MatchStatusMessage
        {
            MatchId = $"{DateTime.UtcNow.Ticks:D}{Guid.NewGuid():N}",
            MatchFinishedEvent = new([1,2,3,4,5], [6,7,8,9,10], DateTime.UtcNow, DateTime.UtcNow.AddMinutes(99))
        };
        var offset = Fixture.Chronicle.MatchStatusConsumer.CommitedOffset;

        // act
        await producer.ProduceAsync(message.MatchId, message, CancellationToken.None);

        // assert
        SpinWait.SpinUntil(() => Fixture.Chronicle.MatchStatusConsumer.CommitedOffset == offset + 1, 1_000);
        await using var scope = Fixture.ChronicleMigrations.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ChronicleDbContext>();
        var entities = await dbContext.Matches
            .Include(x => x.Players)
            .ToListAsync();
        entities.Should().HaveCount(1);
        var dto = entities.First();
        dto.MatchId.Should().Be(message.MatchId);
        dto.StartedAt.Should().BeCloseTo(message.MatchFinishedEvent.StartedAt, TimeSpan.FromSeconds(0.000001)); // kafka drops digits after 1/10^6щящ 
        dto.FinishedAt.Should().BeCloseTo(message.MatchFinishedEvent.FinishedAt, TimeSpan.FromSeconds(0.000001));
        dto.Players.Should().HaveCount(message.MatchFinishedEvent.Winners.Length + message.MatchFinishedEvent.Losers.Length);
        foreach (var player in dto.Players)
            if (player.IsWin)
            {
                message.MatchFinishedEvent.Winners.Should().Contain(player.PlayerId);
                message.MatchFinishedEvent.Losers.Should().NotContain(player.PlayerId);
            }
            else
            {
                message.MatchFinishedEvent.Losers.Should().Contain(player.PlayerId);
                message.MatchFinishedEvent.Winners.Should().NotContain(player.PlayerId);
            }
    }
}