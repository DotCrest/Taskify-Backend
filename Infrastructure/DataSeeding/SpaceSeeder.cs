using Domain.Models;
using Infrastructure.context;

namespace Infrastructure.DataSeeding;

public class SpaceSeeder
{
    private readonly ApplicationDbContext _context;

    public SpaceSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if spaces already exist
        if (_context.Spaces.Any())
            return;

        var workspace = _context.Workspaces.FirstOrDefault();
        if (workspace == null)
            throw new Exception("No workspace found. Seed workspaces first.");

        var spaces = new List<Space>
        {
            new Space
            {
                Name = "Development",
                WorkspaceId = workspace.Id,
                CreatedAt = DateTime.UtcNow,
                IconColor = "#3B82F6",
                IconType = "code",
                IconValue = "dev"
            },
            new Space
            {
                Name = "Design",
                WorkspaceId = workspace.Id,
                CreatedAt = DateTime.UtcNow,
                IconColor = "#EC4899",
                IconType = "palette",
                IconValue = "design"
            },
            new Space
            {
                Name = "Marketing",
                WorkspaceId = workspace.Id,
                CreatedAt = DateTime.UtcNow,
                IconColor = "#F59E0B",
                IconType = "megaphone",
                IconValue = "marketing"
            }
        };

        await _context.Spaces.AddRangeAsync(spaces);
        await _context.SaveChangesAsync();
    }
}
