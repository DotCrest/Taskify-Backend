using Domain.Models;
using Infrastructure.context;

namespace Infrastructure.DataSeeding;

public class GroupSeeder
{
    private readonly ApplicationDbContext _context;

    public GroupSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if groups already exist
        if (_context.Groups.Any())
            return;

        var space = _context.Spaces.FirstOrDefault();
        if (space == null)
            throw new Exception("No space found. Seed spaces first.");

        var groups = new List<Group>
        {
            new Group
            {
                Name = "Sprint 1",
                SpaceId = space.Id,
                CreatedAt = DateTime.UtcNow,
                IconColor = "#3B82F6",
                IconType = "sprint",
                IconValue = "sprint1"
            },
            new Group
            {
                Name = "Backlog",
                SpaceId = space.Id,
                CreatedAt = DateTime.UtcNow,
                IconColor = "#6B7280",
                IconType = "backlog",
                IconValue = "backlog"
            },
            new Group
            {
                Name = "On Hold",
                SpaceId = space.Id,
                CreatedAt = DateTime.UtcNow,
                IconColor = "#F97316",
                IconType = "pause",
                IconValue = "onhold"
            }
        };

        await _context.Groups.AddRangeAsync(groups);
        await _context.SaveChangesAsync();
    }
}
