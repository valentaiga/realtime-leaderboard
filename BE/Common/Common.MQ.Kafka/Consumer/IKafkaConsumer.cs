using Confluent.Kafka;

namespace Common.MQ.Kafka.Consumer;

public interface IKafkaConsumer<TKey, TMessage>
{
    ConsumeResult<TKey, TMessage> Consume(CancellationToken ct);
    void Commit(ConsumeResult<TKey, TMessage> result);
    void Subscribe();
    void Close();
}