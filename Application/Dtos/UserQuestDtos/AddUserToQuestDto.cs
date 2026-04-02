using System.Text.Json.Serialization;

namespace Application.Dtos.UserQuestDtos;

public class AddUserToQuestDto
{
    public string UserId { get; set; } = default!;
    public int QuestId { get; set; }
    [JsonIgnore]
    public int? SpaceId { get; set; }
}
