namespace BackOffice.Identity.Data;

public class UserDto
{
    public const string TableName = "users";
    
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}