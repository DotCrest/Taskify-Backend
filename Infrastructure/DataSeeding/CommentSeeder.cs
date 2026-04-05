using Domain.Models;
using Infrastructure.context;

namespace Infrastructure.DataSeeding;

public class CommentSeeder
{
    private readonly ApplicationDbContext _context;

    public CommentSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if comments already exist
        if (_context.Comments.Any())
            return;

        var quest = _context.Quests.FirstOrDefault();
        var user = _context.Users.FirstOrDefault();

        if (quest == null || user == null)
            throw new Exception("Quest or User not found. Seed quests and users first.");

        var comments = new List<Comment>
        {
            new Comment
            {
                Content = "Started working on this task",
                QuestId = quest.Id,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                UpdatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new Comment
            {
                Content = "Need to review the requirements first",
                QuestId = quest.Id,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                UpdatedAt = DateTime.UtcNow.AddHours(-1)
            },
            new Comment
            {
                Content = "Good progress! Keep up the work",
                QuestId = quest.Id,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await _context.Comments.AddRangeAsync(comments);
        await _context.SaveChangesAsync();
    }
}
