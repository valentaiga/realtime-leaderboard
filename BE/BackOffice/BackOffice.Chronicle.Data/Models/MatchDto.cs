namespace BackOffice.Chronicle.Data.Models;

public class MatchDto
{
    public const string TableName = "matches";
    
    public long Id { get; set; } // different key than MatchId for faster index searches
    public string MatchId { get; set; } = null!;
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }

    public List<MatchPlayerDto> Players { get; set; } = [];
}