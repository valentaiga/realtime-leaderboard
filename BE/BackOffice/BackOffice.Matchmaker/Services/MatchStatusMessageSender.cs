using System.Threading.Channels;
using Common.MQ.Kafka.Producer;
using BackOffice.MQ.Messages.MatchStatus;
using Common.MQ.Primitives;
using Common.Primitives;
using Microsoft.Extensions.Options;

namespace BackOffice.Matchmaker.Services;

public class MatchStatusMessageSender(
    ChannelReader<MatchStatusMessage> channel,
    ObjectRingBuffer<MatchStatusMessage> ringBuffer,
    IKafkaProducer<Guid, MatchStatusMessage> kafkaProducer,
    IOptionsMonitor<MessageSenderOptions> optionsMonitor,
    ILogger<MatchStatusMessageSender> logger) : MessageSenderBase<MatchStatusMessage>(optionsMonitor)
{
    protected override async Task<MatchStatusMessage> ReadMessageAsync() => await channel.ReadAsync();

    protected override async Task SendMessageAsync(MatchStatusMessage message)
    {
        try
        {
            await kafkaProducer.ProduceAsync(message.MatchId, message, CancellationToken.None);
            ringBuffer.Return(message);
        }
        catch (Exception exc)
        {
            logger.LogError(exc, "Failed to send message");
            throw;
        }
    }
}