namespace BackOffice.PlayerSearch.DynamoDb;

public class PlayerDto
{
    public const string TableName = "Players";
    
    public long Id { get; set; }        // partition key
    public string Username { get; set; } = null!;
}