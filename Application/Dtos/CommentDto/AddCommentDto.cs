using System.Text.Json.Serialization;

namespace Application.Dtos.CommentDto;

public class AddCommentDto
{
    public string? UserComment { get; set; }
    public int QuestId { get; set; }
    [JsonIgnore]
    public string? UserId { get; set; }
}
