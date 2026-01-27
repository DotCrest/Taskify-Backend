using Domain.Contracts;

namespace Domain.Models;

public class Category : ISoftDelete
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public int WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = default!;

    public ICollection<Quest> Quests { get; set; } = [];
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
