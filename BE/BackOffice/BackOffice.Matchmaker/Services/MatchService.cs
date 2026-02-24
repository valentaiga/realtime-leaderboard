using System.Threading.Channels;
using Common.MQ.Messages;
using Common.Primitives;

namespace BackOffice.Matchmaker.Services;

public class MatchService(
    ChannelWriter<FinishedMatchMessage> channel,
    ObjectRingBuffer<FinishedMatchMessage> ringBuffer,
    FixedSizeArrayPool<ulong> arrayPool)
{
    public async Task StartMatchAsync(ulong[] players, CancellationToken ct)
    {
        // business logic simplifying: we don't have a GameServer
        // therefore we assume match has finished right after it started with random duration (less than 1h)

        // this binding is not good but works efficient with memory
        var message = ringBuffer.Get();
        message.Winners = arrayPool.Rent();
        message.Losers = arrayPool.Rent();

        message.StartedAt = DateTime.UtcNow;
        message.FinishedAt = message.StartedAt.AddMinutes(Random.Shared.Next(30, 90));
        var playersSpan = players.AsSpan();
        playersSpan[..5].CopyTo(message.Winners);
        playersSpan[5..].CopyTo(message.Losers);
        await channel.WriteAsync(message, ct);
    }
}