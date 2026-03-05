using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Common.MQ.Kafka.Consumer;

public class KafkaConsumer<TKey, TMessage> : IKafkaConsumer<TKey, TMessage>, IDisposable
{
    private readonly ILogger<KafkaConsumer<TKey, TMessage>> _logger;
    private readonly KafkaConsumerConfig _config;
    private readonly IConsumer<TKey, TMessage> _consumer;

    public KafkaConsumer(IDeserializer<TKey>? keyDeserializer,
        IDeserializer<TMessage>? valueDeserializer,
        IOptionsMonitor<KafkaConsumerConfig> optionsMonitor,
        ILogger<KafkaConsumer<TKey, TMessage>> logger)
    {
        _logger = logger;

        _config = optionsMonitor.Get(nameof(TMessage));
        _consumer = new ConsumerBuilder<TKey, TMessage>(_config)
            .SetKeyDeserializer(keyDeserializer)
            .SetValueDeserializer(valueDeserializer)
            .Build();
    }

    public void Subscribe()
    {
        try
        {
            _consumer.Subscribe(_config.Topic);
            _logger.LogInformation("Subscribed to {Topic}", _config.Topic);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Error subscribing to {Topic}", _config.Topic);
            throw;
        }
    }

    public ConsumeResult<TKey, TMessage> Consume(CancellationToken ct)
    {
        var result = _consumer.Consume(ct);
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("Consumed message {@Message} from {Topic} {Partition}", result.Message, result.Topic, result.Partition);
        return result;
    }

    public void Commit(ConsumeResult<TKey, TMessage> result)
    {
        try
        {
            _consumer.Commit(result);
        }
        catch (KafkaException exc)
        {
            _logger.LogWarning(exc, "Failed to commit offset {Offset} for partition {Partition}", result.Offset, result.Partition);
            throw;
        }
    }

    public void Close()
    {
        try
        {
            _consumer.Close();
            _logger.LogInformation("Closed {Topic}", _config.Topic);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Error closing {Topic}", _config.Topic);
            throw;
        }
    }

    public void Dispose()
    {
        Close();
        _consumer.Dispose();
        GC.SuppressFinalize(this);
    }
}