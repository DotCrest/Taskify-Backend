namespace Domain.Models;

public class Workspace
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string? Avatar { get; set; }

    // One-to-Many Relationships (User, Categories, Invitations, Spaces, Tags)
    // Owner (fk)
    public string OwnerId { get; set; } = default!;
    public User User { get; set; } = default!;
    public ICollection<Category> Categories { get; set; } = [];
    public ICollection<Invitation> Invitations { get; set; } = [];
    public ICollection<Space> Spaces { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];
    // Many-to-Many Relationship with Users through WorkspaceMember
    public ICollection<WorkspaceMember> WorkspaceMembers { get; set; } = [];
}
