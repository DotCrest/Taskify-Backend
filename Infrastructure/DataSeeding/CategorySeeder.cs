using Domain.Models;
using Infrastructure.context;

namespace Infrastructure.DataSeeding;

public class CategorySeeder
{
    private readonly ApplicationDbContext _context;

    public CategorySeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if categories already exist
        if (_context.Categories.Any())
            return;

        var workspace = _context.Workspaces.FirstOrDefault();
        if (workspace == null)
            throw new Exception("No workspace found. Seed workspaces first.");

        var categories = new List<Category>
        {
            new Category
            {
                Name = "Bug Fix",
                WorkspaceId = workspace.Id
            },
            new Category
            {
                Name = "Feature",
                WorkspaceId = workspace.Id
            },
            new Category
            {
                Name = "Documentation",
                WorkspaceId = workspace.Id
            },
            new Category
            {
                Name = "Refactoring",
                WorkspaceId = workspace.Id
            },
            new Category
            {
                Name = "Research",
                WorkspaceId = workspace.Id
            }
        };

        await _context.Categories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
    }
}
