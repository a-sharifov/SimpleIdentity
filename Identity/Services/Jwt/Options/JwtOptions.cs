namespace Identity.Jwt.Options;

public class JwtOptions
{
    public string Issuer { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public int RefreshTokenExpirationTimeMinutes { get; set; }
    public int TokenExpirationTimeMinutes { get; set; }
}
