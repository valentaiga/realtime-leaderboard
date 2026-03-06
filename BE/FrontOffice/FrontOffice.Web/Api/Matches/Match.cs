namespace FrontOffice.Web.Api.Matches;

public class Match
{
    public string MatchId { get; set; } = null!;

    public DateTime StartedAt { get; set; }

    public DateTime FinishedAt { get; set; }

    public IEnumerable<MatchPlayer> Players { get; set; } = [];
}

public class MatchPlayer
{
    public long PlayerId { get; set; }
    public bool IsWin { get; set; }
}