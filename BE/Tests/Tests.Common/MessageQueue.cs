using System.Threading.Channels;

namespace Tests.Common;

public interface IMessageQueue<TMessage>
{
    Task<TMessage> ReadAsync(CancellationToken ct);
    Task WriteAsync(TMessage message, CancellationToken ct);
}

public class SharedMessageQueue<TMessage> : IMessageQueue<TMessage>
{
    private static readonly Channel<TMessage> Channel = System.Threading.Channels.Channel.CreateUnbounded<TMessage>(new UnboundedChannelOptions
    {
        AllowSynchronousContinuations = true
    });

    public Task<TMessage> ReadAsync(CancellationToken ct) =>
        Channel.Reader.ReadAsync(ct).AsTask();

    public Task WriteAsync(TMessage message, CancellationToken ct) =>
        Channel.Writer.WriteAsync(message, ct).AsTask();
}

public class LocalMessageQueue<TMessage> : IMessageQueue<TMessage>
{
    private readonly Channel<TMessage> _channel = Channel.CreateUnbounded<TMessage>();

    public Task<TMessage> ReadAsync(CancellationToken ct) =>
        _channel.Reader.ReadAsync(ct).AsTask();

    public Task WriteAsync(TMessage message, CancellationToken ct) =>
        _channel.Writer.WriteAsync(message, ct).AsTask();
}