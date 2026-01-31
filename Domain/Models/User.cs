using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class User : IdentityUser
{
    public string Name { get; set; } = null!;
    public string? PhotoUrl { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Relations: One to Many => relationships with (Quests, Workspaces, Comments, Invitations)
    public ICollection<Quest> CreatedQuests { get; set; } = []; // As Author
    public ICollection<UserQuest> AssignedQuests { get; set; } = []; // As Assignee in UserQuest table
    public ICollection<Workspace> OwnedWorkspaces { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<Invitation> SentInvitations { get; set; } = [];

    // Relations: Many-to-Many => relationships with (Space, Workspace, Quests)
    public ICollection<SpaceMember> SpaceMembers { get; set; } = [];
    public ICollection<WorkspaceMember> WorkspaceMembers { get; set; } = [];
}