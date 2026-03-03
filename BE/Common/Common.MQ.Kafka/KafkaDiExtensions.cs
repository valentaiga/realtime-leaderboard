using System.Diagnostics.CodeAnalysis;
using System.Net;
using Common.MQ.Kafka.Consumer;
using Common.MQ.Kafka.Producer;

namespace Common.MQ.Kafka;

public static class KafkaDiExtensions
{
    public static IServiceCollection AddKafkaProducer<TKey, TMessage>(
        this IServiceCollection services,
        IConfiguration configuration,
        string configSectionPath,
        Action<KafkaProducerConfig>? configure = null)
    {
        services.AddOptions<KafkaProducerConfig>(nameof(TMessage))
            .Configure(options =>
            {
                configuration.GetSection(configSectionPath).Bind(options);
                options.ClientId = Dns.GetHostName();
                configure?.Invoke(options);

                if (string.IsNullOrWhiteSpace(options.Topic))
                    throw new KeyNotFoundException($"{nameof(options.Topic)} not set from configuration ({configSectionPath}).");
            });

        services.AddSingleton<IKafkaProducer<TKey, TMessage>, KafkaProducer<TKey, TMessage>>();
        return services;
    }

    public static IServiceCollection AddKafkaConsumer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TConsumer, TKey, TMessage>(
        this IServiceCollection services,
        IConfiguration configuration,
        string configSectionPath,
        Action<KafkaConsumerConfig>? configure = null)
        where TConsumer : KafkaConsumerBase<TKey, TMessage>
    {
        services.AddOptions<KafkaConsumerConfig>(nameof(TMessage))
            .Configure(options =>
            {
                configuration.GetSection(configSectionPath).Bind(options);
                configure?.Invoke(options);

                if (string.IsNullOrWhiteSpace(options.ClientId))
                    throw new KeyNotFoundException($"{nameof(options.ClientId)} not set from configuration ({configSectionPath}).");
                if (string.IsNullOrWhiteSpace(options.Topic))
                    throw new KeyNotFoundException($"{nameof(options.Topic)} not set from configuration ({configSectionPath}).");
            });

        services.AddHostedService<TConsumer>();
        return services;
    }
}