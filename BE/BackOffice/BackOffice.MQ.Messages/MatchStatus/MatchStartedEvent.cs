using MessagePack;

namespace BackOffice.MQ.Messages.MatchStatus;

[MessagePackObject]
public class MatchStartedEvent(long[] team1, long[] team2, DateTime startedAt)
{
    [Key("team1")]
    public long[] Team1 { get; } = team1;

    [Key("team2")]
    public long[] Team2 { get; } = team2;

    [Key("startedAt")]
    public DateTime StartedAt { get; } = startedAt;
}