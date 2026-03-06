namespace Common.MQ.Kafka.Configurator;

internal class KafkaInitializationService(IKafkaTopicCreator creator, IEnumerable<TopicConfiguration> configurations) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => creator.CreateTopicsAsync(configurations);

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}