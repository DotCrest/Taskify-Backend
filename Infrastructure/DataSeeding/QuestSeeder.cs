using Domain.Models;
using Infrastructure.context;

namespace Infrastructure.DataSeeding;

public class QuestSeeder
{
    private readonly ApplicationDbContext _context;

    public QuestSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if quests already exist
        if (_context.Quests.Any())
            return;

        var space = _context.Spaces.FirstOrDefault();
        var author = _context.Users.FirstOrDefault(u => u.Email == "admin@taskify.com");
        var category = _context.Categories.FirstOrDefault();

        if (space == null || author == null)
            throw new Exception("Space or Author not found. Seed spaces and users first.");

        var quests = new List<Quest>
        {
            new Quest
            {
                Title = "Setup project repository",
                Description = "Initialize the project repository with proper structure and documentation",
                SpaceId = space.Id,
                CategoryId = category?.Id,
                AuthorId = author.Id,
                Status = QuestStatusEnum.Complete,
                Priority = PriorityEnum.High,
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                UpdatedAt = DateTime.UtcNow.AddDays(-5),
                DueDate = DateTime.UtcNow.AddDays(-2)
            },
            new Quest
            {
                Title = "Design API endpoints",
                Description = "Create RESTful API endpoints design documentation",
                SpaceId = space.Id,
                CategoryId = category?.Id,
                AuthorId = author.Id,
                Status = QuestStatusEnum.InProgress,
                Priority = PriorityEnum.High,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(3)
            },
            new Quest
            {
                Title = "Implement authentication",
                Description = "Add JWT authentication to the API",
                SpaceId = space.Id,
                CategoryId = category?.Id,
                AuthorId = author.Id,
                Status = QuestStatusEnum.Todo,
                Priority = PriorityEnum.Urgent,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                DueDate = DateTime.UtcNow.AddDays(5)
            },
            new Quest
            {
                Title = "Write unit tests",
                Description = "Write comprehensive unit tests for API layer",
                SpaceId = space.Id,
                CategoryId = category?.Id,
                AuthorId = author.Id,
                Status = QuestStatusEnum.Todo,
                Priority = PriorityEnum.Normal,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                DueDate = DateTime.UtcNow.AddDays(7)
            },
            new Quest
            {
                Title = "Database optimization",
                Description = "Optimize database queries and add proper indexes",
                SpaceId = space.Id,
                CategoryId = category?.Id,
                AuthorId = author.Id,
                Status = QuestStatusEnum.Todo,
                Priority = PriorityEnum.Low,
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14)
            }
        };

        await _context.Quests.AddRangeAsync(quests);
        await _context.SaveChangesAsync();
    }
}
