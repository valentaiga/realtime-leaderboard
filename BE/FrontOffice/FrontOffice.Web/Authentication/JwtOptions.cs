namespace FrontOffice.Web.Authentication;

public sealed class JwtOptions
{
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required TimeSpan TokenExpiration { get; set; }
    public required TimeSpan RefreshTokenExpiration { get; set; }
}