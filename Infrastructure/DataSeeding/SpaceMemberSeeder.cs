using Domain.Models;
using Infrastructure.context;

namespace Infrastructure.DataSeeding;

public class SpaceMemberSeeder
{
    private readonly ApplicationDbContext _context;

    public SpaceMemberSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if space members already exist
        if (_context.SpaceMembers.Any())
            return;

        var space = _context.Spaces.FirstOrDefault();
        var users = _context.Users.ToList();

        if (space == null || !users.Any())
            throw new Exception("No space or users found. Seed spaces and users first.");

        var spaceMembers = new List<SpaceMember>();

        // Add first two users as space members
        for (int i = 0; i < Math.Min(2, users.Count); i++)
        {
            spaceMembers.Add(new SpaceMember
            {
                SpaceId = space.Id,
                UserId = users[i].Id,
                Role = users[i].Email == "admin@taskify.com" ? "admin" : "member",
                JoinedAt = DateTime.UtcNow
            });
        }

        await _context.SpaceMembers.AddRangeAsync(spaceMembers);
        await _context.SaveChangesAsync();
    }
}
