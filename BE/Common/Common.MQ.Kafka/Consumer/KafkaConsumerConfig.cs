using Confluent.Kafka;

namespace Common.MQ.Kafka.Consumer;

public class KafkaConsumerConfig : ConsumerConfig
{
    public string Topic { get; set; } = null!;
}