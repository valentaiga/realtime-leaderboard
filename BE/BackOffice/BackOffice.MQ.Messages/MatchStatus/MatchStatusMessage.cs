using Common.Primitives;
using MessagePack;

namespace BackOffice.MQ.Messages.MatchStatus;

[MessagePackObject]
public class MatchStatusMessage : IClearable
{
    [Key("matchId")]
    public string MatchId { get; set; } = null!;

    [Key("matchStartedEvent")]
    public MatchStartedEvent? MatchStartedEvent { get; set; }

    [Key("matchFinishedEvent")]
    public MatchFinishedEvent? MatchFinishedEvent { get; set; }

    public void Clear()
    {
        MatchId = null!;
        MatchStartedEvent = null;
        MatchFinishedEvent = null;
    }
}