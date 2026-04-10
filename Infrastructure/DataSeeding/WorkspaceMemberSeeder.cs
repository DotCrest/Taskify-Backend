using Domain.Models;
using Infrastructure.context;

namespace Infrastructure.DataSeeding;

public class WorkspaceMemberSeeder
{
    private readonly ApplicationDbContext _context;

    public WorkspaceMemberSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if workspace members already exist
        if (_context.WorkspaceMembers.Any())
            return;

        var workspace = _context.Workspaces.FirstOrDefault();
        var users = _context.Users.ToList();

        if (workspace == null || !users.Any())
            throw new Exception("No workspace or users found. Seed workspaces and users first.");

        var workspaceMembers = new List<WorkspaceMember>();

        // Add all users as workspace members
        foreach (var user in users)
        {
            workspaceMembers.Add(new WorkspaceMember
            {
                WorkspaceId = workspace.Id,
                UserId = user.Id,
                Role = user.Email == "admin@taskify.com" ? "admin" : "member",
                JoinedAt = DateTime.UtcNow
            });
        }

        await _context.WorkspaceMembers.AddRangeAsync(workspaceMembers);
        await _context.SaveChangesAsync();
    }
}
