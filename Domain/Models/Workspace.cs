using Domain.Contracts;

namespace Domain.Models;

public class Workspace : ISoftDelete
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string? Avatar { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Owner (fk)
    public string OwnerId { get; set; } = default!;
    public User User { get; set; } = default!;

    // One-to-Many Relationships
    public ICollection<Category> Categories { get; set; } = [];
    public ICollection<Invitation> Invitations { get; set; } = [];
    public ICollection<Space> Spaces { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];
    // Many-to-Many Relationship with Users through WorkspaceMember
    public ICollection<WorkspaceMember> WorkspaceMembers { get; set; } = [];
}
