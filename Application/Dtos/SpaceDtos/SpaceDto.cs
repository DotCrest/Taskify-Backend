namespace Application.Dtos.SpaceDtos;

public class SpaceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? IconColor { get; set; }
    public string? IconType { get; set; }
    public string? IconValue { get; set; }
    public int WorkspaceId { get; set; }
    // TODO: create a List of QuestDto to return the space related quests.
}
