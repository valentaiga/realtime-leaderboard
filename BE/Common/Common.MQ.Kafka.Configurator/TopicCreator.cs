using Confluent.Kafka;

namespace Common.MQ.Kafka.Configurator;

internal class TopicCreator(IEnumerable<TopicConfiguration> options, ILogger<TopicCreator> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var groups = options.GroupBy(x => x.BootstrapServers);
        foreach (var configurationGroup in groups)
        {
            var bootstrapServers = configurationGroup.Key;

            logger.LogDebug("Creating kafka admin client for {BootstrapServers}", bootstrapServers);
            using var client = CreateClient(bootstrapServers);

            logger.LogDebug("Getting kafka metadata from {BootstrapServers}", bootstrapServers);
            var metadata = client.GetMetadata(TimeSpan.FromSeconds(10));

            var topicsToCreate = configurationGroup
                .Where(x => !metadata.Topics.Exists(m => m.Topic == x.Name)).ToArray();

            if (topicsToCreate.Length == 0)
                continue;

            logger.LogInformation("Creating {@Topics} topics on {BootstrapServers} servers...", topicsToCreate, bootstrapServers);

            await client.CreateTopicsAsync(topicsToCreate);
            logger.LogInformation("Created {TopicsCount} topics on {BootstrapServers} servers", topicsToCreate.Length, bootstrapServers);
        }
    }

    private static IAdminClient CreateClient(string bootstrapServers)
    {
        KeyValuePair<string, string>[] config =
        [
            new("bootstrap.servers", bootstrapServers)
        ];

        return new AdminClientBuilder(config).Build();
    }
}