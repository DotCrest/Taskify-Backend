using Domain.Contracts;

namespace Domain.Models;

public class UserQuest : ISoftDelete
{
    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;

    public int QuestId { get; set; }
    public Quest Quest { get; set; } = default!;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
