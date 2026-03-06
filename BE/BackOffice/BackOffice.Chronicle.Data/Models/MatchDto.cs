namespace BackOffice.Chronicle.Data.Models;

public class MatchDto
{
    public const string TableName = "matches";
    
    public long Id { get; set; } // different key than MatchId, since MatchId can be repeated (N/2^32 chance)
    public Guid MatchId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }

    public List<MatchPlayerDto> Players { get; set; } = [];
}