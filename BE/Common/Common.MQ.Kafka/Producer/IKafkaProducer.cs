namespace Common.MQ.Kafka.Producer;

public interface IKafkaProducer<in TKey, in TMessage>
{
    Task ProduceAsync(TKey key, TMessage message, CancellationToken ct);
}