using Confluent.Kafka;

namespace Tests.Common.Kafka;

public class TestKafkaConsumer<TKey, TValue>(MemoryMessageQueue<ConsumeResult<TKey, TValue>> memoryQueue) : IConsumer<TKey, TValue>
{
    public ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken = default) =>
        memoryQueue.ReadAsync(cancellationToken).GetAwaiter().GetResult();

    public void Commit(ConsumeResult<TKey, TValue> result)
    {
    }

    public void Subscribe(string topic)
    {
    }

    public void Unsubscribe()
    {
    }

    public void Subscribe(IEnumerable<string> topics) => throw new NotImplementedException();

    public void Assign(TopicPartition partition) => throw new NotImplementedException();

    public void Assign(TopicPartitionOffset partition) => throw new NotImplementedException();

    public void Assign(IEnumerable<TopicPartitionOffset> partitions) => throw new NotImplementedException();

    public void Assign(IEnumerable<TopicPartition> partitions) => throw new NotImplementedException();

    public void IncrementalAssign(IEnumerable<TopicPartitionOffset> partitions) => throw new NotImplementedException();

    public void IncrementalAssign(IEnumerable<TopicPartition> partitions) => throw new NotImplementedException();

    public void IncrementalUnassign(IEnumerable<TopicPartition> partitions) => throw new NotImplementedException();

    public void Unassign() => throw new NotImplementedException();

    public void StoreOffset(ConsumeResult<TKey, TValue> result) => throw new NotImplementedException();

    public void StoreOffset(TopicPartitionOffset offset) => throw new NotImplementedException();

    public List<TopicPartitionOffset> Commit() => throw new NotImplementedException();

    public void Commit(IEnumerable<TopicPartitionOffset> offsets) => throw new NotImplementedException();

    public void Seek(TopicPartitionOffset tpo) => throw new NotImplementedException();

    public void Pause(IEnumerable<TopicPartition> partitions) => throw new NotImplementedException();

    public void Resume(IEnumerable<TopicPartition> partitions) => throw new NotImplementedException();

    public List<TopicPartitionOffset> Committed(TimeSpan timeout) => throw new NotImplementedException();

    public List<TopicPartitionOffset> Committed(IEnumerable<TopicPartition> partitions, TimeSpan timeout) => throw new NotImplementedException();

    public Offset Position(TopicPartition partition) => throw new NotImplementedException();

    public List<TopicPartitionOffset> OffsetsForTimes(IEnumerable<TopicPartitionTimestamp> timestampsToSearch, TimeSpan timeout) => throw new NotImplementedException();

    public WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition) => throw new NotImplementedException();

    public WatermarkOffsets QueryWatermarkOffsets(TopicPartition topicPartition, TimeSpan timeout) => throw new NotImplementedException();

    public int AddBrokers(string brokers) => throw new NotImplementedException();

    public void SetSaslCredentials(string username, string password) => throw new NotImplementedException();

    public ConsumeResult<TKey, TValue> Consume(TimeSpan timeout)
    {
        throw new NotImplementedException();
    }
    
    public Handle Handle { get; }
    public string Name { get; }
    public ConsumeResult<TKey, TValue> Consume(int millisecondsTimeout)
    {
        throw new NotImplementedException();
    }


    public void Close()
    {
    }

    public string MemberId { get; } = null!;
    public List<TopicPartition> Assignment { get; } = [];
    public List<string> Subscription { get; } = [];
    public IConsumerGroupMetadata ConsumerGroupMetadata { get; } = null!;

    public void Dispose() => throw new NotImplementedException();
}