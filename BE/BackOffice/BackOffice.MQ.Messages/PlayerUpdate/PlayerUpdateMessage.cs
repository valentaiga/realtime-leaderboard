using Common.Primitives;
using MessagePack;

namespace BackOffice.MQ.Messages.PlayerUpdate;

[MessagePackObject]
public class PlayerUpdateMessage : IClearable
{
    [Key("playerId")]
    public long PlayerId { get; set; }

    [Key("playerEloChangedEvent")]
    public PlayerEloChangedEvent? PlayerEloChangedEvent { get; set; }

    public void Clear()
    {
        PlayerId = 0;
        PlayerEloChangedEvent = null;
    }
}