using Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class User : IdentityUser, ISoftDelete
{
    public string Name { get; set; } = null!;
    public string? PhotoUrl { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Relation 1: One to Many => relationships with (Quests, Workspaces, Comments, Invitations)
    public ICollection<Quest> CreatedQuests { get; set; } = [];
    public ICollection<Workspace> OwnedWorkspaces { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<Invitation> SentInvitations { get; set; } = [];

    // Relation 2: Many-to-Many => Quests assigned to these users 
    public ICollection<UserQuest> AssignedQuests { get; set; } = [];
    // Relation 3: many-to-many => relationship between Users and Spaces
    public ICollection<SpaceMember> SpaceMembers { get; set; } = [];
    // Relation 4: many-to-many => relationship between Users and Workspaces
    public ICollection<WorkspaceMember> WorkspaceMembers { get; set; } = [];
    public ICollection<RefeshToken>? RefeshTokens { get; set; }
}