namespace Domain.Models;

public class WorkspaceMember
{
    public int WorkspaceId { get; set; }
    public string UserId { get; set; } = default!;
    public string Role { get; set; } = default!;
    public DateTime JoinedAt { get; set; }

    // One-to-Many Relationships (Workspace, User)
    public Workspace Workspace { get; set; } = default!;
    public User User { get; set; } = default!;
}
