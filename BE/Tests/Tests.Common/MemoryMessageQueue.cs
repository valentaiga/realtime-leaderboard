using System.Threading.Channels;

namespace Tests.Common;

public class MemoryMessageQueue<TMessage>
{
    private static readonly Channel<TMessage> Channel = System.Threading.Channels.Channel.CreateUnbounded<TMessage>();

    public Task<TMessage> ReadAsync(CancellationToken ct) =>
        Channel.Reader.ReadAsync(ct).AsTask();

    public Task WriteAsync(TMessage message, CancellationToken ct) =>
        Channel.Writer.WriteAsync(message, ct).AsTask();
}