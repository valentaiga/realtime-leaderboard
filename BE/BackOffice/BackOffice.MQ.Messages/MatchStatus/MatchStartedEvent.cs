using MessagePack;

namespace BackOffice.MQ.Messages.MatchStatus;

[MessagePackObject]
public class MatchStartedEvent(ulong[] team1, ulong[] team2, DateTime startedAt)
{
    [Key(0)]
    public ulong[] Team1 { get; } = team1;

    [Key(1)]
    public ulong[] Team2 { get; } = team2;

    [Key(2)]
    public DateTime StartedAt { get; } = startedAt;
}