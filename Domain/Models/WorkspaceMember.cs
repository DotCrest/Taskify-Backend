using Domain.Contracts;

namespace Domain.Models;

public class WorkspaceMember : ISoftDelete
{
    public int WorkspaceId { get; set; }
    public string UserId { get; set; } = default!;
    public string Role { get; set; } = default!;
    public DateTime JoinedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Workspace Workspace { get; set; } = default!;
    public User User { get; set; } = default!;
}
