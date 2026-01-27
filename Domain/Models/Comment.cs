using Domain.Contracts;

namespace Domain.Models;

public class Comment : ISoftDelete
{
    public int Id { get; set; }
    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;

    public int QuestId { get; set; }
    public Quest Quest { get; set; } = default!;
}
