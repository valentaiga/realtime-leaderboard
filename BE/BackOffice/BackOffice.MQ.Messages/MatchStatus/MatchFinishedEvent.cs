using MessagePack;

namespace BackOffice.MQ.Messages.MatchStatus;

[MessagePackObject]
public class MatchFinishedEvent(long[] winners, long[] losers, DateTime startedAt, DateTime finishedAt)
{
    [Key(0)]
    public long[] Winners { get; } = winners;

    [Key(1)]
    public long[] Losers { get; } = losers;

    [Key(2)]
    public DateTime StartedAt { get; } = startedAt;

    [Key(3)]
    public DateTime FinishedAt { get; } = finishedAt;
}