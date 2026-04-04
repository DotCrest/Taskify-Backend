namespace Application.Dtos.CommentDto;

public class CommentDto
{
    public string Id { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public int QuestId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsUpdated => UpdatedAt.HasValue;
}
