using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Common.MQ.Kafka.Consumer;

public abstract class KafkaConsumerBase<TKey, TMessage> : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IConsumer<TKey, TMessage> _consumer;
    private readonly string _topic;

    protected KafkaConsumerBase(IDeserializer<TKey> keyDeserializer, IDeserializer <TMessage> valueDeserializer, IOptionsMonitor<KafkaConsumerConfig> optionsMonitor, ILogger logger)
    {
        _logger = logger;
        var config = optionsMonitor.Get(nameof(TMessage));
        _topic = config.Topic;
        _consumer = new ConsumerBuilder<TKey, TMessage>(config)
            .SetKeyDeserializer(keyDeserializer)
            .SetValueDeserializer(valueDeserializer)
            .Build();
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        try
        {
            _consumer.Subscribe(_topic);
            while (!ct.IsCancellationRequested)
            {
                var result = _consumer.Consume(ct);
                await ConsumeAsync(result, ct);
            }

            _consumer.Close();
            _consumer.Dispose();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Error in consumer loop");
        }
    }

    /// <inheritdoc cref="IConsumer{TKey,TValue}.Commit()"/>
    protected void Commit(ConsumeResult<TKey, TMessage> result) => _consumer.Commit(result);

    /// <inheritdoc cref="IConsumer{TKey,TValue}.Consume(CancellationToken)"/>
    protected abstract Task ConsumeAsync(ConsumeResult<TKey, TMessage> result, CancellationToken ct);
}