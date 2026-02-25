using System.Threading.Channels;
using BackOffice.MQ.Messages.MatchStatus;
using Common.Primitives;

namespace BackOffice.Matchmaker.Services;

public class MatchService(
    ObjectRingBuffer<MatchStatusMessage> ringBuffer,
    ChannelWriter<MatchStatusMessage> channel)
{
    public async Task StartMatchAsync(ulong[] players, CancellationToken ct)
    {
        // business logic simplifying: we don't have a GameServer
        // therefore we assume match has finished right after it started with random duration (less than 1h)
        var matchStartedMessage = ringBuffer.Get();
        matchStartedMessage.MatchId = Guid.NewGuid();
        matchStartedMessage.MatchStartedEvent = new(players[..5].ToArray(), players[5..].ToArray(), DateTime.UtcNow);
        await channel.WriteAsync(matchStartedMessage, ct);

        var matchFinishedMessage = ringBuffer.Get();
        matchFinishedMessage.MatchId = matchStartedMessage.MatchId;
        matchFinishedMessage.MatchFinishedEvent = new(
            matchStartedMessage.MatchStartedEvent.Team1,
            matchStartedMessage.MatchStartedEvent.Team2,
            matchStartedMessage.MatchStartedEvent.StartedAt,
            matchStartedMessage.MatchStartedEvent.StartedAt.AddMinutes(Random.Shared.Next(30, 90)));
        await channel.WriteAsync(matchFinishedMessage, ct);
    }
}