namespace Application.Dtos.UserQuestDtos;

public class UserToQuestDto
{
    public string UserId { get; set; } = default!;
    public int QuestId { get; set; }
    public int WorkspaceId { get; set; }
}
