using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Common.MQ.Kafka.Consumer;

public abstract class KafkaConsumerBase<TKey, TMessage> : BackgroundService
{
    private readonly IConsumer<TKey, TMessage> _consumer;
    private readonly string _topic;

    protected KafkaConsumerBase(IOptionsMonitor<KafkaConsumerConfig> optionsMonitor)
    {
        var config = optionsMonitor.Get(nameof(TMessage));
        _topic = config.Topic;
        _consumer = new ConsumerBuilder<TKey, TMessage>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _consumer.Subscribe(_topic);
        while (!ct.IsCancellationRequested)
        {
            var result = _consumer.Consume(ct);
            await ConsumeAsync(result, ct);
        }
        _consumer.Close();
        _consumer.Dispose();
    }

    protected abstract Task ConsumeAsync(ConsumeResult<TKey, TMessage> result, CancellationToken ct);
}