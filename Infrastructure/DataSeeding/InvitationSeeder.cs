using Domain.Models;
using Infrastructure.context;

namespace Infrastructure.DataSeeding;

public class InvitationSeeder
{
    private readonly ApplicationDbContext _context;

    public InvitationSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if invitations already exist
        if (_context.Invitations.Any())
            return;

        var workspace = _context.Workspaces.FirstOrDefault();
        var space = _context.Spaces.FirstOrDefault();
        var sender = _context.Users.FirstOrDefault(u => u.Email == "admin@taskify.com");

        if (workspace == null || sender == null)
            throw new Exception("No workspace or sender found. Seed workspaces and users first.");

        var invitations = new List<Invitation>
        {
            new Invitation
            {
                ReceiverEmail = "newuser@taskify.com",
                WorkspaceId = workspace.Id,
                SpaceId = space?.Id,
                SenderId = sender.Id,
                Status = InvitationStatusEnum.Pending,
                ReceiverRole = "member",
                Token = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            },
            new Invitation
            {
                ReceiverEmail = "anotheruser@taskify.com",
                WorkspaceId = workspace.Id,
                SenderId = sender.Id,
                Status = InvitationStatusEnum.Pending,
                ReceiverRole = "member",
                Token = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Invitation
            {
                ReceiverEmail = "accepted@taskify.com",
                WorkspaceId = workspace.Id,
                SenderId = sender.Id,
                Status = InvitationStatusEnum.Accepted,
                ReceiverRole = "member",
                Token = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        await _context.Invitations.AddRangeAsync(invitations);
        await _context.SaveChangesAsync();
    }
}
