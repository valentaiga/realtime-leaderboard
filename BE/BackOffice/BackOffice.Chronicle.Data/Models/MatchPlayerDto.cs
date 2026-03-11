namespace BackOffice.Chronicle.Data.Models;

public class MatchPlayerDto
{
    public const string TableName = "match_players";

    public long MatchId { get; set; }
    public long PlayerId { get; set; }
    public bool IsWin { get; set; }
    public int? EloChange { get; set; }
}