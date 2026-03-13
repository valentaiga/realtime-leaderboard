using MessagePack;

namespace BackOffice.MQ.Messages.Player;

[MessagePackObject]
public class PlayerRegisteredEvent
{
    [Key("username")]
    public string Username { get; set; } = null!;
}