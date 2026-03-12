using MessagePack;

namespace BackOffice.MQ.Messages.PlayerUpdate;

[MessagePackObject]
public class PlayerEloChangedEvent
{
    [Key("matchId")]
    public string MatchId { get; set; } = null!;

    [Key("eloChange")]
    public byte EloChange { get; set; }
}