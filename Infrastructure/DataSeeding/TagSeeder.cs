using Domain.Models;
using Infrastructure.context;

namespace Infrastructure.DataSeeding;

public class TagSeeder
{
    private readonly ApplicationDbContext _context;

    public TagSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if tags already exist
        if (_context.Tags.Any())
            return;

        var workspace = _context.Workspaces.FirstOrDefault();
        if (workspace == null)
            throw new Exception("No workspace found. Seed workspaces first.");

        var tags = new List<Tag>
        {
            new Tag
            {
                Name = "Urgent",
                Color = "#EF4444",
                WorkspaceId = workspace.Id
            },
            new Tag
            {
                Name = "Important",
                Color = "#F97316",
                WorkspaceId = workspace.Id
            },
            new Tag
            {
                Name = "In Progress",
                Color = "#3B82F6",
                WorkspaceId = workspace.Id
            },
            new Tag
            {
                Name = "Blocked",
                Color = "#8B5CF6",
                WorkspaceId = workspace.Id
            },
            new Tag
            {
                Name = "Review",
                Color = "#06B6D4",
                WorkspaceId = workspace.Id
            },
            new Tag
            {
                Name = "Testing",
                Color = "#10B981",
                WorkspaceId = workspace.Id
            }
        };

        await _context.Tags.AddRangeAsync(tags);
        await _context.SaveChangesAsync();
    }
}
