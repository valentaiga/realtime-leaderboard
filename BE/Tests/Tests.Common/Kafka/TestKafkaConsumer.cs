using Common.MQ.Kafka.Consumer;
using Confluent.Kafka;

namespace Tests.Common.Kafka;

public class TestKafkaConsumer<TKey, TValue>(MemoryMessageQueue<ConsumeResult<TKey, TValue>> memoryQueue) : IKafkaConsumer<TKey, TValue>
{
    private bool _subscribed;
    public long CommitedOffset => _commitedOffset;
    private long _commitedOffset = 0;

    public ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken = default)
    {
        if (!_subscribed)
            throw new Exception("Not subscribed for topic read");
        return memoryQueue.ReadAsync(cancellationToken).GetAwaiter().GetResult();
    }

    public void Commit(ConsumeResult<TKey, TValue> result)
    {
        _commitedOffset = result.Offset;
    }

    public void Subscribe() => _subscribed = true;

    public void Close() => _subscribed = false;
}