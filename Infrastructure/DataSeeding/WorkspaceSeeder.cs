using Domain.Models;
using Infrastructure.context;

namespace Infrastructure.DataSeeding;

public class WorkspaceSeeder
{
    private readonly ApplicationDbContext _context;

    public WorkspaceSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if workspaces already exist
        if (_context.Workspaces.Any())
            return;

        var adminUser = _context.Users.FirstOrDefault(u => u.Email == "admin@taskify.com");
        if (adminUser == null)
            throw new Exception("Admin user not found. Seed users first.");

        var workspaces = new List<Workspace>
        {
            new Workspace
            {
                Name = "Personal Workspace",
                OwnerId = adminUser.Id,
                CreatedAt = DateTime.UtcNow,
                Avatar = null
            },
            new Workspace
            {
                Name = "Team Workspace",
                OwnerId = adminUser.Id,
                CreatedAt = DateTime.UtcNow,
                Avatar = null
            }
        };

        await _context.Workspaces.AddRangeAsync(workspaces);
        await _context.SaveChangesAsync();
    }
}
