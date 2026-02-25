namespace Common.MQ.Kafka.Configurator;

public static class KafkaDiExtensions
{
    public static IServiceCollection ConfigureKafka(this IServiceCollection services, IConfiguration configuration, Action<KafkaConfigurator> configure)
    {
        var builder = new KafkaConfigurator(services, configuration);
        configure.Invoke(builder);
        return builder.Build();
    }
}
