using Confluent.Kafka.Admin;

namespace Common.MQ.Kafka.Configurator;

public class TopicConfiguration : TopicSpecification
{
    public string BootstrapServers { get; set; } = null!;
}