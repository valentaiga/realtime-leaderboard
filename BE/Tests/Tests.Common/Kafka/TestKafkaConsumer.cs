using Common.MQ.Kafka.Consumer;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Tests.Common.Kafka;

public class TestKafkaConsumer<TKey, TValue>(IMessageQueue<ConsumeResult<TKey, TValue>> memoryQueue, ILogger<TestKafkaProducer<TKey, TValue>> logger) : IKafkaConsumer<TKey, TValue>
{
    private bool _subscribed;
    public long CommitedOffset => _commitedOffset;
    private long _commitedOffset = 0;

    public ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken = default)
    {
        if (!_subscribed)
            throw new Exception("Not subscribed for topic read");
        var result = memoryQueue.ReadAsync(cancellationToken).GetAwaiter().GetResult();
        logger.LogInformation("Consumed message {@Message}", result.Message);
        return result;
    }

    public void Commit(ConsumeResult<TKey, TValue> result) => Interlocked.Exchange(ref _commitedOffset, result.Offset);

    public void Subscribe() => _subscribed = true;

    public void Close() => _subscribed = false;
}