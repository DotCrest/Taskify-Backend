using Microsoft.AspNetCore.Identity;

namespace Domain.Entites;

public class User : IdentityUser
{
    public string Name { get; set; } = null!;
    public string? PhotoUrl { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public List<RefeshToken>? RefeshTokens { get; set; }

}
