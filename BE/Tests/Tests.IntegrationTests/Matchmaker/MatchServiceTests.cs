using BackOffice.Matchmaker.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.IntegrationTests.Matchmaker;

[Collection(TestConstants.TestCollections.IntegrationTests)]
public class MatchServiceTests(IntegrationTestFixture fixture) : IntegrationTestBase(fixture)
{
    private readonly MatchService _service = fixture.Mm.Services.GetRequiredService<MatchService>();

    [Fact]
    public async Task StartMatch_ProducesMatchFinishedEvent()
    {
        // arrange
        var producer = Fixture.Mm.MatchStatusMessageKafkaProducer;
        producer.ResetSavedMessages();
        var players = Enumerable.Range(0, 10).Select(x => (long)x).ToArray();

        // act
        await _service.StartMatchAsync(players, CancellationToken.None);

        // assert
        SpinWait.SpinUntil(() => producer.ProducedMessages.Count == 2, 1_000);
        var producedMessage = producer.ProducedMessages.Should().ContainSingle(x => x.Message.Value.MatchFinishedEvent != null).Which;
        producedMessage.Message.Key.Should().Be(producedMessage.Message.Value.MatchId);
        var @event = producedMessage.Message.Value.MatchFinishedEvent;
        @event.Should().NotBeNull();
        @event.Winners.Concat(@event.Losers).Should().BeEquivalentTo(players);
    }
}