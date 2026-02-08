using System.Text.Json.Serialization;

namespace Application.Dtos;

public class AuthResponseDto
{
    public string Message { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public List<string>? Roles { get; set; }
    public string? Token { get; set; }
    public DateTime? ExpiresOn { get; set; }
    [JsonIgnore]
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpiration { get; set; }
}
