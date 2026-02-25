using MessagePack;

namespace BackOffice.MQ.Messages.MatchStatus;

[MessagePackObject]
public class MatchStatusMessage
{
    [Key(0)]
    public Guid MatchId { get; set; }

    [Key(1)]
    public MatchStartedEvent? MatchStartedEvent { get; set; }

    [Key(2)]
    public MatchFinishedEvent? MatchFinishedEvent { get; set; }
}