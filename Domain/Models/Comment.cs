namespace Domain.Models;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    // Relations: One to Many => relationships with (User, Quest)
    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;

    public int QuestId { get; set; }
    public Quest Quest { get; set; } = default!;
}
