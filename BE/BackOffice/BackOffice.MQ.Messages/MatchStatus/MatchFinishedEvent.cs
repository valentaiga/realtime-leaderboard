using MessagePack;

namespace BackOffice.MQ.Messages.MatchStatus;

[MessagePackObject]
public class MatchFinishedEvent(ulong[] winners, ulong[] losers, DateTime startedAt, DateTime finishedAt)
{
    [Key(0)]
    public ulong[] Winners { get; } = winners;

    [Key(1)]
    public ulong[] Losers { get; } = losers;

    [Key(2)]
    public DateTime StartedAt { get; } = startedAt;

    [Key(3)]
    public DateTime FinishedAt { get; } = finishedAt;
}