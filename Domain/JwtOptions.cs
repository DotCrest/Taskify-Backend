namespace Domain;

public class JwtOptions
{

    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public double ExpirationInDays { get; set; }
}
