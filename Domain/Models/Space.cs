namespace Domain.Models;

public class Space
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public string? IconColor { get; set; }
    public string? IconType { get; set; }
    public string? IconValue { get; set; }

    // (fk) Workspace
    public int WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = default!;
    /// Relationship: One-To-Many with (Quests, Groups, Invitations, UserSpace)    
    public ICollection<Quest> Quests { get; set; } = [];
    public ICollection<Group> Groups { get; set; } = [];
    public ICollection<Invitation> Invitations { get; set; } = [];
    public ICollection<SpaceMember> SpaceMembers { get; set; } = [];
}
