using Domain.Models;
using Infrastructure.context;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.DataSeeding;

/// <summary>
/// Master seeder class that orchestrates all data seeding operations.
/// Ensures proper execution order to handle foreign key dependencies.
/// </summary>
public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DataSeeder(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Seeds all data in the correct order, respecting foreign key relationships.
    /// </summary>
    public async Task SeedAllAsync()
    {
        try
        {
            // Order matters! Seed in dependency order:
            // 0. Roles (foundational for user management)
            // 1. Users (base entity)
            // 1.5. User Roles (assign users to roles)
            // 2. Workspaces (depends on User)
            // 3. Spaces (depends on Workspace)
            // 4. Categories (depends on Workspace)
            // 5. Tags (depends on Workspace)
            // 6. Groups (depends on Space)
            // 7. Quests (depends on Space, Category, User)
            // 8. Comments (depends on Quest, User)
            // 9. UserQuests (depends on User, Quest)
            // 10. WorkspaceMembers (depends on Workspace, User)
            // 11. SpaceMembers (depends on Space, User)
            // 12. Invitations (depends on Workspace, Space, User)
            // 13. VerificationCodes (standalone)

            Console.WriteLine("Starting data seeding...");

            var roleSeeder = new RoleSeeder(_roleManager);
            Console.WriteLine("Seeding roles...");
            await roleSeeder.SeedAsync();

            var userSeeder = new UserSeeder(_userManager);
            Console.WriteLine("Seeding users...");
            await userSeeder.SeedAsync();

            var userRoleSeeder = new UserRoleSeeder(_userManager);
            Console.WriteLine("Seeding user roles...");
            await userRoleSeeder.SeedAsync();

            var workspaceSeeder = new WorkspaceSeeder(_context);
            Console.WriteLine("Seeding workspaces...");
            await workspaceSeeder.SeedAsync();

            var spaceSeeder = new SpaceSeeder(_context);
            Console.WriteLine("Seeding spaces...");
            await spaceSeeder.SeedAsync();

            var categorySeeder = new CategorySeeder(_context);
            Console.WriteLine("Seeding categories...");
            await categorySeeder.SeedAsync();

            var tagSeeder = new TagSeeder(_context);
            Console.WriteLine("Seeding tags...");
            await tagSeeder.SeedAsync();

            var groupSeeder = new GroupSeeder(_context);
            Console.WriteLine("Seeding groups...");
            await groupSeeder.SeedAsync();

            var questSeeder = new QuestSeeder(_context);
            Console.WriteLine("Seeding quests...");
            await questSeeder.SeedAsync();

            var commentSeeder = new CommentSeeder(_context);
            Console.WriteLine("Seeding comments...");
            await commentSeeder.SeedAsync();

            var userQuestSeeder = new UserQuestSeeder(_context);
            Console.WriteLine("Seeding user quests...");
            await userQuestSeeder.SeedAsync();

            var workspaceMemberSeeder = new WorkspaceMemberSeeder(_context);
            Console.WriteLine("Seeding workspace members...");
            await workspaceMemberSeeder.SeedAsync();

            var spaceMemberSeeder = new SpaceMemberSeeder(_context);
            Console.WriteLine("Seeding space members...");
            await spaceMemberSeeder.SeedAsync();

            var invitationSeeder = new InvitationSeeder(_context);
            Console.WriteLine("Seeding invitations...");
            await invitationSeeder.SeedAsync();

            var verificationCodeSeeder = new VerificationCodeSeeder(_context);
            Console.WriteLine("Seeding verification codes...");
            await verificationCodeSeeder.SeedAsync();

            Console.WriteLine("? Data seeding completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Error during data seeding: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Seeds only a specific entity type.
    /// Useful for incremental seeding or testing.
    /// </summary>
    public async Task SeedSpecificAsync(string entityType)
    {
        try
        {
            switch (entityType.ToLower())
            {
                case "role":
                    var roleSeeder = new RoleSeeder(_roleManager);
                    await roleSeeder.SeedAsync();
                    break;
                case "user":
                    var userSeeder = new UserSeeder(_userManager);
                    await userSeeder.SeedAsync();
                    break;
                case "userrole":
                    var userRoleSeeder = new UserRoleSeeder(_userManager);
                    await userRoleSeeder.SeedAsync();
                    break;
                case "workspace":
                    var workspaceSeeder = new WorkspaceSeeder(_context);
                    await workspaceSeeder.SeedAsync();
                    break;
                case "space":
                    var spaceSeeder = new SpaceSeeder(_context);
                    await spaceSeeder.SeedAsync();
                    break;
                case "category":
                    var categorySeeder = new CategorySeeder(_context);
                    await categorySeeder.SeedAsync();
                    break;
                case "tag":
                    var tagSeeder = new TagSeeder(_context);
                    await tagSeeder.SeedAsync();
                    break;
                case "group":
                    var groupSeeder = new GroupSeeder(_context);
                    await groupSeeder.SeedAsync();
                    break;
                case "quest":
                    var questSeeder = new QuestSeeder(_context);
                    await questSeeder.SeedAsync();
                    break;
                case "comment":
                    var commentSeeder = new CommentSeeder(_context);
                    await commentSeeder.SeedAsync();
                    break;
                case "userquest":
                    var userQuestSeeder = new UserQuestSeeder(_context);
                    await userQuestSeeder.SeedAsync();
                    break;
                case "workspacemember":
                    var workspaceMemberSeeder = new WorkspaceMemberSeeder(_context);
                    await workspaceMemberSeeder.SeedAsync();
                    break;
                case "spacemember":
                    var spaceMemberSeeder = new SpaceMemberSeeder(_context);
                    await spaceMemberSeeder.SeedAsync();
                    break;
                case "invitation":
                    var invitationSeeder = new InvitationSeeder(_context);
                    await invitationSeeder.SeedAsync();
                    break;
                case "verificationcode":
                    var verificationCodeSeeder = new VerificationCodeSeeder(_context);
                    await verificationCodeSeeder.SeedAsync();
                    break;
                default:
                    throw new ArgumentException($"Unknown entity type: {entityType}");
            }

            Console.WriteLine($"? {entityType} seeding completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Error during {entityType} seeding: {ex.Message}");
            throw;
        }
    }
}
