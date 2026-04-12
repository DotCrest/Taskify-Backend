using Domain.Models;
using Infrastructure.context;

namespace Infrastructure.DataSeeding;

public class UserQuestSeeder
{
    private readonly ApplicationDbContext _context;

    public UserQuestSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if user quests already exist
        if (_context.UserQuests.Any())
            return;

        var quests = _context.Quests.ToList();
        var users = _context.Users.ToList();

        if (!quests.Any() || !users.Any())
            throw new Exception("No quests or users found. Seed quests and users first.");

        var userQuests = new List<UserQuest>();

        // Assign first quest to first user
        if (quests.Count > 0 && users.Count > 0)
        {
            userQuests.Add(new UserQuest
            {
                UserId = users[0].Id,
                QuestId = quests[0].Id
            });
        }

        // Assign second quest to multiple users
        if (quests.Count > 1 && users.Count > 1)
        {
            userQuests.Add(new UserQuest
            {
                UserId = users[0].Id,
                QuestId = quests[1].Id
            });
            userQuests.Add(new UserQuest
            {
                UserId = users[1].Id,
                QuestId = quests[1].Id
            });
        }

        // Assign third quest to second user
        if (quests.Count > 2 && users.Count > 1)
        {
            userQuests.Add(new UserQuest
            {
                UserId = users[1].Id,
                QuestId = quests[2].Id
            });
        }

        // Assign fourth quest to third user
        if (quests.Count > 3 && users.Count > 2)
        {
            userQuests.Add(new UserQuest
            {
                UserId = users[2].Id,
                QuestId = quests[3].Id
            });
        }

        await _context.UserQuests.AddRangeAsync(userQuests);
        await _context.SaveChangesAsync();
    }
}
