using BackOffice.MQ.Messages.MatchStatus;
using Common.MQ.Kafka.Consumer;

namespace BackOffice.Chronicle;

// todo vm: add kafka DLQ
public class MatchStatusHandler(
    MatchService matchService,
    IKafkaConsumer<Guid, MatchStatusMessage> kafkaConsumer,
    ILogger<MatchStatusHandler> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        kafkaConsumer.Subscribe();

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var result = kafkaConsumer.Consume(ct);
                if (result.Message.Value.MatchFinishedEvent is { } matchFinishedEvent)
                    await matchService.SaveFinishedMatchAsync(result.Message.Key, matchFinishedEvent, ct);

                kafkaConsumer.Commit(result);
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
}