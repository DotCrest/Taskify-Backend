namespace Domain.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public int WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = default!;
    // Relations: One to Many => relationships with (Quests)
    public ICollection<Quest> Quests { get; set; } = [];
}
