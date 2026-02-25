namespace Common.MQ.Kafka.Configurator;

public class KafkaConfigurator
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;
    private readonly IList<TopicConfiguration> _topics = [];

    public KafkaConfigurator(IServiceCollection services, IConfiguration configuration)
    {
        _services = services;
        _configuration = configuration;
    }

    public KafkaConfigurator CreateTopic(string configSectionPath)
    {
        var topicSpecification = new TopicConfiguration();
        _configuration.GetSection(configSectionPath).Bind(topicSpecification);
        return CreateTopic(topicSpecification);
    }

    public KafkaConfigurator CreateTopic(TopicConfiguration topicSpecification)
    {
        _topics.Add(topicSpecification);
        return this;
    }

    internal IServiceCollection Build()
    {
        _services.AddHostedService(sp => new TopicCreator(
            _topics,
            sp.GetRequiredService<ILogger<TopicCreator>>()));
        return _services;
    }
}