using MessagePack;

namespace BackOffice.MQ.Messages.MatchStatus;

[MessagePackObject]
public class MatchFinishedEvent(long[] winners, long[] losers, DateTime startedAt, DateTime finishedAt)
{
    [Key("winners")]
    public long[] Winners { get; } = winners;

    [Key("losers")]
    public long[] Losers { get; } = losers;

    [Key("startedAt")]
    public DateTime StartedAt { get; } = startedAt;

    [Key("finishedAt")]
    public DateTime FinishedAt { get; } = finishedAt;
}