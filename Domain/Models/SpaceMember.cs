namespace Domain.Models;

public class SpaceMember
{
    public int SpaceId { get; set; }
    public string UserId { get; set; } = default!;
    public string Role { get; set; } = default!;
    public DateTime JoinedAt { get; set; }
    // Relations: One to Many => relationships with (User, Space)
    public User User { get; set; } = default!;
    public Space Space { get; set; } = default!;
}
