using System.Text.Json;
using Common.MQ.Kafka.Producer;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Tests.Common.Kafka;

public class TestKafkaProducer<TKey, TMessage>(
    IOptionsMonitor<KafkaProducerConfig> optionsMonitor,
    IMessageQueue<ConsumeResult<TKey, TMessage>> messageQueue,
    ILogger<TestKafkaProducer<TKey, TMessage>> logger) : IKafkaProducer<TKey, TMessage>
{
    public static TestKafkaProducer<TKey, TMessage> CreateInstance(string topic)
    {
        var optionsMonitor = new PredefinedOptionsMonitor<KafkaProducerConfig>(new KafkaProducerConfig()
        {
            Topic = topic
        });
        return new TestKafkaProducer<TKey, TMessage>(optionsMonitor, new SharedMessageQueue<ConsumeResult<TKey, TMessage>>(), NullLogger<TestKafkaProducer<TKey, TMessage>>.Instance);
    }

    public IReadOnlyList<ConsumeResult<TKey, TMessage>> ProducedMessages => _producedMessages;

    private readonly List<ConsumeResult<TKey, TMessage>> _producedMessages = [];
    private readonly string _topic = optionsMonitor.Get(nameof(TMessage)).Topic
        ?? throw new Exception("Configuration for producer is incorrect");

    private long _offset = 0;

    public Task ProduceAsync(TKey key, TMessage message, CancellationToken ct)
    {
        logger.LogInformation("Produced message {@Message}", message);
        
        
        var result = new ConsumeResult<TKey, TMessage>
        {
            Offset = Interlocked.Increment(ref _offset),
            Message = new Message<TKey, TMessage> { Key = key, Value = message },
            Topic = _topic,
        };

        // copying message because some services return them to ObjectPool after success produce
        result.Message = JsonSerializer.Deserialize<Message<TKey, TMessage>>(JsonSerializer.Serialize(result.Message))
            ?? throw new InvalidOperationException("Something went wrong in TestKafkaProducer");
        
        _producedMessages.Add(result);
        return messageQueue.WriteAsync(result, ct);
    }

    public void ResetSavedMessages() => _producedMessages.Clear();
}