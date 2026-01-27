using Domain.Contracts;

namespace Domain.Models;

public class SpaceMember : ISoftDelete
{
    public int SpaceId { get; set; }
    public string UserId { get; set; } = default!;
    public string Role { get; set; } = default!;
    public DateTime JoinedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public User User { get; set; } = default!;
    public Space Space { get; set; } = default!;
}
