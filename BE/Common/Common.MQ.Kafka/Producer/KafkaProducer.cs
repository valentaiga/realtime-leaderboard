using Common.Primitives;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Common.MQ.Kafka.Producer;

public sealed class KafkaProducer<TKey, TMessage> : IKafkaProducer<TKey, TMessage>, IDisposable
{
    private readonly ObjectRingBuffer<Message<TKey, TMessage>> _ringBuffer = new();
    private readonly string _topic;
    private readonly IProducer<TKey, TMessage> _producer;

    public KafkaProducer(ISerializer<TKey> keySerializer, ISerializer<TMessage> valueSerializer, IOptionsMonitor<KafkaProducerConfig> optionsMonitor)
    {
        var config = optionsMonitor.Get(nameof(TMessage));
        _topic = config.Topic;
        _producer = new ProducerBuilder<TKey, TMessage>(config)
            .SetKeySerializer(keySerializer)
            .SetValueSerializer(valueSerializer)
            .Build();
    }

    public async Task ProduceAsync(TKey key, TMessage message, CancellationToken ct)
    {
        var kafkaMessage = _ringBuffer.Get();
        kafkaMessage.Key = key;
        kafkaMessage.Value = message;
        await _producer.ProduceAsync(_topic, kafkaMessage, ct);
        _ringBuffer.Return(kafkaMessage); // return message to buffer only on success
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}