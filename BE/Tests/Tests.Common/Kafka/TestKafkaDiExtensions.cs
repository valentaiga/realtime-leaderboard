using Common.MQ.Kafka.Consumer;
using Common.MQ.Kafka.Producer;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tests.Common.Kafka;

public static class TestKafkaDiExtensions
{
    public static IServiceCollection ReplaceKafkaConsumerWithInMemoryQueue<TKey, TValue>(this IServiceCollection provider)
    {
        provider
            .RemoveAll<IKafkaConsumer<TKey, TValue>>()
            .AddSingleton<IKafkaConsumer<TKey, TValue>, TestKafkaConsumer<TKey, TValue>>()
            .TryAddSingleton<IMessageQueue<ConsumeResult<TKey, TValue>>, SharedMessageQueue<ConsumeResult<TKey, TValue>>>();
        return provider;
    }

    public static IServiceCollection ReplaceKafkaProducerWithInMemoryQueue<TKey, TValue>(this IServiceCollection provider)
    {
        provider
            .RemoveAll<IKafkaProducer<TKey, TValue>>()
            .AddSingleton<IKafkaProducer<TKey, TValue>, TestKafkaProducer<TKey, TValue>>()
            .TryAddSingleton<IMessageQueue<ConsumeResult<TKey, TValue>>, SharedMessageQueue<ConsumeResult<TKey, TValue>>>();
        return provider;
    }
}