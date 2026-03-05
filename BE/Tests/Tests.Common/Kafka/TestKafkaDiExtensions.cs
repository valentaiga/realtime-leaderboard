using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tests.Common.Kafka;

public static class TestKafkaDiExtensions
{
    public static IServiceCollection ReplaceKafkaConsumerWithInMemoryQueue<TKey, TValue>(this IServiceCollection provider)
    {
        provider
            .RemoveAll<IConsumer<TKey, TValue>>()
            .AddSingleton<IConsumer<TKey, TValue>, TestKafkaConsumer<TKey, TValue>>()
            .TryAddSingleton<MemoryMessageQueue<ConsumeResult<TKey, TValue>>>();
        return provider;
    }

    public static IServiceCollection ReplaceKafkaProducerWithInMemoryQueue<TKey, TValue>(this IServiceCollection provider)
    {
        provider
            .RemoveAll<IProducer<TKey, TValue>>()
            .AddSingleton<IProducer<TKey, TValue>, TestKafkaProducer<TKey, TValue>>()
            .TryAddSingleton<MemoryMessageQueue<ConsumeResult<TKey, TValue>>>();
        return provider;
    }
}