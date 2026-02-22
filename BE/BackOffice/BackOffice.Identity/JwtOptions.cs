namespace BackOffice.Identity;

public sealed class JwtOptions
{
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required int ExpirationHours { get; set; }
}