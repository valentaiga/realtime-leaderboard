using Confluent.Kafka;

namespace Common.MQ.Kafka.Producer;

public class KafkaProducerConfig : ProducerConfig
{
    public string Topic { get; set; } = null!;
}