namespace Application.Dtos.UserQuestDtos;

public class UserToQuestDto
{
    public string AssigneeId { get; set; } = default!;
    public int QuestId { get; set; }
    public int SpaceId { get; set; }
    public int WorkspaceId { get; set; }
    public string? AssignerId { get; set; }
}
