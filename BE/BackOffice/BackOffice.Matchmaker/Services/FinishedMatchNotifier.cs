using System.Threading.Channels;
using Common.MQ.Messages;
using Common.MQ.Primitives;
using Common.Primitives;
using Microsoft.Extensions.Options;

namespace BackOffice.Matchmaker.Services;

public class FinishedMatchNotifier(
    ChannelReader<FinishedMatchMessage> channel,
    IOptionsMonitor<MessageSenderOptions> optionsMonitor,
    ObjectRingBuffer<FinishedMatchMessage> ringBuffer,
    FixedSizeArrayPool<ulong> arrayPool,
    ILogger<FinishedMatchNotifier> logger) : MessageSender<FinishedMatchMessage>(optionsMonitor)
{
    protected override async Task<FinishedMatchMessage> ReadMessageAsync() => await channel.ReadAsync();

    protected override Task OnSendMessageErrorAsync(Exception exception, FinishedMatchMessage message)
    {
        logger.LogError(exception, "Failed to send message");
        return Task.CompletedTask;
    }

    protected override Task OnMessageSentAsync(FinishedMatchMessage message)
    {
        ringBuffer.Return(message);
        arrayPool.Return(message.Losers);
        arrayPool.Return(message.Winners);
        return Task.CompletedTask;
    }
}