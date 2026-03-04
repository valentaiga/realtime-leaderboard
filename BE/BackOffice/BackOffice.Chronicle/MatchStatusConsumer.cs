using BackOffice.MQ.Messages.MatchStatus;
using Common.MQ.Kafka.Consumer;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace BackOffice.Chronicle;

// todo vm: add kafka DLQ
public class MatchStatusConsumer(
    MatchService matchService,
    IDeserializer<Guid> keyDeserializer,
    IDeserializer<MatchStatusMessage> valueDeserializer,
    IOptionsMonitor<KafkaConsumerConfig> optionsMonitor,
    ILogger<MatchStatusConsumer> logger)
    : KafkaConsumerBase<Guid, MatchStatusMessage>(keyDeserializer, valueDeserializer, optionsMonitor, logger)
{
    protected override async Task ConsumeAsync(ConsumeResult<Guid, MatchStatusMessage> result, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
            try
            {
                // saving only finished matches
                if (result.Message.Value.MatchFinishedEvent is {} matchFinishedEvent)
                {
                    if (logger.IsEnabled(LogLevel.Debug))
                        logger.LogDebug("Received match finished event {MatchId}", result.Message.Key);

                    await matchService.SaveFinishedMatchAsync(result.Message.Key, matchFinishedEvent, ct);
                }

                try
                {
                    Commit(result);
                }
                catch (KafkaException exc)
                {
                    logger.LogWarning(exc, "Failed to commit offset {Offset} for partition {Partition}", result.Offset, result.Partition);
                    throw;
                }

                break;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An exception occurred while consuming the message.");
                await Task.Delay(1_000, ct);
            }
    }
}