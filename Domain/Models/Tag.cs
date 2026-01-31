namespace Domain.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Color { get; set; } = default!;

    // Relations: One to Many => relationships with (Workspace)
    public int WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = default!;

    // Relations: Many-to-Many => relationships with (Quests)
    public ICollection<Quest> Quests { get; set; } = [];
}
