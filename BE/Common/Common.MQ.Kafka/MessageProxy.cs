using Common.Primitives;
using Confluent.Kafka;

namespace Common.MQ.Kafka;

public sealed class MessageProxy<TKey, TValue> : Message<TKey, TValue>, IClearable
{
    public void Clear()
    {
        Value = default!;
        Key = default!;
        Headers = [];
        Timestamp = Timestamp.Default;
    }
}