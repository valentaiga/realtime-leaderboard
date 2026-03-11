using Common.Primitives;

namespace BackOffice.MQ.Messages.PlayerUpdate;

public class PlayerUpdateMessage : IClearable
{
    public long PlayerId { get; set; }
    public PlayerEloChangedEvent? PlayerEloChangedEvent { get; set; }

    public void Clear()
    {
        PlayerId = 0;
        PlayerEloChangedEvent = null;
    }
}