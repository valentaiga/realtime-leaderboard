namespace BackOffice.MQ.Messages.PlayerUpdate;

public class PlayerEloChangedEvent
{
    public string MatchId { get; set; } = null!;
    public byte EloChange { get; set; }
}