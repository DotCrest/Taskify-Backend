namespace Domain.Models;

public class UserQuest
{
    // Relations: One to Many => relationships with (User, Quest)
    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;
    public int QuestId { get; set; }
    public Quest Quest { get; set; } = default!;
}
