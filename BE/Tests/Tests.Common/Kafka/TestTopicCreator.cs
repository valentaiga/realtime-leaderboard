using Common.MQ.Kafka.Configurator;
using Microsoft.Extensions.Logging;

namespace Tests.Common.Kafka;

public class TestTopicCreator(ILogger<TestTopicCreator> logger) : IKafkaTopicCreator
{
    public Task CreateTopicsAsync(IEnumerable<TopicConfiguration> configurations)
    {
        foreach (var configuration in configurations)
            logger.LogInformation("Topic {Topic} created on {BootstrapServers}", configuration.Name, configuration.BootstrapServers);
        return Task.CompletedTask;
    }
}