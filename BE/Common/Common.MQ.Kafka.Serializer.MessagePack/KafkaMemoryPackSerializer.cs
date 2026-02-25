using System.Runtime.Serialization;
using Confluent.Kafka;
using MessagePack;

namespace Common.MQ.Kafka.Serializer.MessagePack;

public class KafkaMemoryPackSerializer<TValue>(MessagePackSerializerOptions memoryPackOptions) : ISerializer<TValue>
{
    public byte[] Serialize(TValue data, SerializationContext context)
    {
        try
        {
            return MessagePackSerializer.Serialize(data, memoryPackOptions);
        }
        catch (Exception ex)
        {
            throw new SerializationException($"Failed to serialize message of type {typeof(TValue).Name}", ex);
        }
    }
}