using Domain.Contracts;

namespace Domain.Models;

public class Group : ISoftDelete // this is a List of Quests but, for clean code I named it 'Group' not 'List'
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public string? IconColor { get; set; }
    public string? IconType { get; set; }
    public string? IconValue { get; set; }

    public int SpaceId { get; set; }
    public Space Space { get; set; } = default!;

    public ICollection<Quest> Quests { get; set; } = [];
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
