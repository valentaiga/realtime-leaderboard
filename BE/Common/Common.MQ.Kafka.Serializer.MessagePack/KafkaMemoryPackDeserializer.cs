using System.Buffers;
using System.Runtime.Serialization;
using Confluent.Kafka;
using MessagePack;

namespace Common.MQ.Kafka.Serializer.MessagePack;

public class KafkaMemoryPackDeserializer<TValue>(MessagePackSerializerOptions memoryPackOptions) : IDeserializer<TValue>
{
    public TValue Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            return default!;

        try
        {
            using var memory = MemoryPool<byte>.Shared.Rent(data.Length);
            data.CopyTo(memory.Memory.Span);
            return MessagePackSerializer.Deserialize<TValue>(memory.Memory[..data.Length], memoryPackOptions);
        }
        catch (Exception ex)
        {
            throw new SerializationException($"Failed to deserialize message of type {typeof(TValue).Name}", ex);
        }
    }
}