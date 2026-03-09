using Common.Primitives;
using MessagePack;

namespace BackOffice.MQ.Messages.MatchStatus;

[MessagePackObject]
public class MatchStatusMessage : IClearable
{
    [Key(0)]
    public string MatchId { get; set; } = null!;

    [Key(1)]
    public MatchStartedEvent? MatchStartedEvent { get; set; }

    [Key(2)]
    public MatchFinishedEvent? MatchFinishedEvent { get; set; }

    public void Clear()
    {
        MatchId = null!;
        MatchStartedEvent = null;
        MatchFinishedEvent = null;
    }
}