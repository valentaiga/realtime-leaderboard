using Common.MQ.Kafka.Producer;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Tests.Common.Kafka;

public class TestKafkaProducer<TKey, TMessage>(IOptionsMonitor<KafkaProducerConfig> optionsMonitor, MemoryMessageQueue<ConsumeResult<TKey, TMessage>> memoryQueue) : IKafkaProducer<TKey, TMessage>
{
    private long _offset = 0;
    private readonly string _topic = optionsMonitor.Get(nameof(TMessage)).Topic
        ?? throw new Exception("Configuration for producer is incorrect");

    public Task ProduceAsync(TKey key, TMessage message, CancellationToken ct)
    {
        return memoryQueue.WriteAsync(new ConsumeResult<TKey, TMessage>
        {
            Offset = new Offset(Interlocked.Increment(ref _offset)),
            Message = new Message<TKey, TMessage> { Key = key, Value = message },
            Topic = _topic,
        }, ct);
    }
}