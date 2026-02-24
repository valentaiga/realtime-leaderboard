namespace Common.MQ.Messages;

public class FinishedMatchMessage
{
    public ulong[] Winners { get; set; } = null!;
    public ulong[] Losers { get; set; } = null!;
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
}