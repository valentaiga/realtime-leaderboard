using Common.Primitives;
using MessagePack;

namespace BackOffice.MQ.Messages.Player;

[MessagePackObject]
public class PlayerMessage : IClearable
{
    [Key("playerId")]
    public long PlayerId { get; set; }

    [Key("playerEloChangedEvent")]
    public PlayerEloChangedEvent? PlayerEloChangedEvent { get; set; }

    [Key("playerRegisteredEvent")]
    public PlayerRegisteredEvent? PlayerRegisteredEvent { get; set; }

    public void Clear()
    {
        PlayerId = 0;
        PlayerEloChangedEvent = null;
        PlayerRegisteredEvent = null;
    }
}