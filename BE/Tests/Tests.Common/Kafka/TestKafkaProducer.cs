using Common.MQ.Kafka.Producer;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tests.Common.Kafka;

public class TestKafkaProducer<TKey, TMessage>(
    IOptionsMonitor<KafkaProducerConfig> optionsMonitor,
    IMessageQueue<ConsumeResult<TKey, TMessage>> messageQueue,
    ILogger<TestKafkaProducer<TKey, TMessage>> logger) : IKafkaProducer<TKey, TMessage>
{
    private long _offset = 0;
    private readonly string _topic = optionsMonitor.Get(nameof(TMessage)).Topic
        ?? throw new Exception("Configuration for producer is incorrect");

    public Task ProduceAsync(TKey key, TMessage message, CancellationToken ct)
    {
        logger.LogInformation("Produced message {@Message}", message);
        return messageQueue.WriteAsync(new ConsumeResult<TKey, TMessage>
        {
            Offset = new Offset(Interlocked.Increment(ref _offset)),
            Message = new Message<TKey, TMessage> { Key = key, Value = message },
            Topic = _topic,
        }, ct);
    }
}