using System.Threading.Channels;
using BackOffice.MQ.Messages.Player;
using Common.MQ.Kafka.Producer;
using Common.MQ.Primitives;
using Microsoft.Extensions.Options;

namespace BackOffice.Identity.Identity;

public class PlayerRegisteredEventSender(
    ChannelReader<PlayerMessage> channel,
    IKafkaProducer<long, PlayerMessage> kafkaProducer,
    IOptionsMonitor<MessageSenderOptions> optionsMonitor,
    ILogger<PlayerRegisteredEventSender> logger) : MessageSenderBase<PlayerMessage>(optionsMonitor)
{
    protected override async Task<PlayerMessage> ReadMessageAsync(CancellationToken ct) => await channel.ReadAsync(ct);

    protected override async Task SendMessageAsync(PlayerMessage message)
    {
        try
        {
            await kafkaProducer.ProduceAsync(message.PlayerId, message, CancellationToken.None);
        }
        catch (Exception exc)
        {
            logger.LogError(exc, "Failed to send message");
            throw;
        }
    }
}