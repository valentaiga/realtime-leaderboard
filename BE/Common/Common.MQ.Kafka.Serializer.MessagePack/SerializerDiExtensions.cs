using System.Diagnostics.CodeAnalysis;
using Confluent.Kafka;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.MQ.Kafka.Serializer.MessagePack;

public static class SerializerDiExtensions
{
    public static IServiceCollection AddMemoryPackKafkaSerializer(this IServiceCollection services, IFormatterResolver messagesResolver)
    {
        var options = MessagePackSerializerOptions.Standard.WithResolver(
            CompositeResolver.Create(
                StandardResolver.Instance,
                messagesResolver,
                BuiltinResolver.Instance));
        services.TryAddSingleton(options);
        services.AddSingleton(typeof(ISerializer<>), typeof(KafkaMemoryPackSerializer<>));
        return services;
    }

    public static IServiceCollection AddMemoryPackKafkaDeserializer(this IServiceCollection services, IFormatterResolver messagesResolver)
    {
        var options = MessagePackSerializerOptions.Standard.WithResolver(
            CompositeResolver.Create(
                StandardResolver.Instance,
                messagesResolver,
                BuiltinResolver.Instance));
        services.TryAddSingleton(options);
        services.AddSingleton(typeof(IDeserializer<>), typeof(KafkaMemoryPackDeserializer<>));
        return services;
    }

    /// <summary> Methods is recommended for NativeAOT applications. </summary>
    /// <remarks> Native code to support creating generic services might not be available with native AOT. </remarks>
    public static IServiceCollection OverrideKafkaSerializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TSerializer, TValue>(this IServiceCollection services)
        where TSerializer : class, ISerializer<TValue>
        where TValue : struct =>
        services.AddSingleton<ISerializer<TValue>, TSerializer>();

    /// <summary> Methods is recommended for NativeAOT applications. </summary>
    /// <remarks> Native code to support creating generic services might not be available with native AOT. </remarks>
    public static IServiceCollection OverrideKafkaDeserializer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TDeserializer, TValue>(this IServiceCollection services)
        where TDeserializer : class, IDeserializer<TValue>
        where TValue : struct =>
        services.AddSingleton<IDeserializer<TValue>, TDeserializer>();
}
