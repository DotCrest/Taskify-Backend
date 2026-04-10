using System.Text.Json.Serialization;

namespace Application.Dtos.CommentDto;

public class UpdateCommentDto
{
    public string? UserComment { get; set; }
    public int CommentId { get; set; }
    [JsonIgnore]
    public string? UserId { get; set; }
}
