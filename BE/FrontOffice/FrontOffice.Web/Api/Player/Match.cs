namespace FrontOffice.Web.Api.Player;

public class Match
{
    public ulong[] Winners { get; set; } = [];

    public ulong[] Losers { get; set; } = [];

    public DateTime StartedAt { get; set; }

    public DateTime FinishedAt { get; set; }
}