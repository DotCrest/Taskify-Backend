namespace Domain.Models;

public class Quest
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }

    public QuestStatusEnum Status { get; set; }
    public PriorityEnum Priority { get; set; }

    // Relations: One to Many => relationships with (Category, Group, User)
    // Relationships
    public int? CategoryId { get; set; }
    public Category? Category { get; set; } = default!;

    public int GroupId { get; set; }
    public Group Group { get; set; } = default!;

    // (Creator)
    public string? AuthorId { get; set; } = default!;
    public User? Author { get; set; } = default!;

    // Relations: One-to-Many => relationships with (UserQuests)
    public ICollection<UserQuest> Assignees { get; set; } = [];

    // Relations: Many-to-Many => relationships with (User, Tag)
    public ICollection<Tag> Tags { get; set; } = [];
}
