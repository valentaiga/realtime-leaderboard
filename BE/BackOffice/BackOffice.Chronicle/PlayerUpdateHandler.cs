using BackOffice.MQ.Messages.PlayerUpdate;
using Common.MQ.Kafka.Consumer;

namespace BackOffice.Chronicle;

public class PlayerUpdateHandler(
    MatchService matchService,
    IKafkaConsumer<long, PlayerUpdateMessage> kafkaConsumer,
    ILogger<MatchStatusHandler> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        kafkaConsumer.Subscribe();

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var result = kafkaConsumer.Consume(ct);
                if (result.Message.Value.PlayerEloChangedEvent is { } playerEloChangedEvent)
                    await matchService.UpdatePlayerEloChangeAsync(result.Message.Key, playerEloChangedEvent, ct);

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

        kafkaConsumer.Close();
    }
}