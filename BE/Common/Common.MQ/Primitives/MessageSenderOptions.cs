namespace Common.MQ.Primitives;

public sealed class MessageSenderOptions
{
    public TimeSpan ReadMessageErrorRetryInterval { get; set; } = TimeSpan.FromSeconds(1);
    public TimeSpan SendMessageErrorRetryInterval { get; set; } = TimeSpan.FromSeconds(1);
}