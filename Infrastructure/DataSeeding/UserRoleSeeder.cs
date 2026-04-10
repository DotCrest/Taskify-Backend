using Domain.Constants;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.DataSeeding;

/// <summary>
/// Seeder class responsible for assigning seeded users to their respective roles.
/// Maps users to roles (e.g., admin user to Admin role, regular users to Member role).
/// </summary>
public class UserRoleSeeder
{
    private readonly UserManager<User> _userManager;

    public UserRoleSeeder(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Seeds user-role assignments into the database.
    /// Assigns:
    /// - admin@taskify.com to Admin role
    /// - Other users to Member role
    /// Only assigns roles if the user doesn't already have them.
    /// </summary>
    public async Task SeedAsync()
    {
        // Define user-role mappings
        var userRoleMappings = new Dictionary<string, string[]>
        {
            { "admin@taskify.com", new[] { Role.Admin } },
            { "john@taskify.com", new[] { Role.Member } },
            { "jane@taskify.com", new[] { Role.Member } }
        };

        foreach (var (email, roles) in userRoleMappings)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                continue; // Skip if user doesn't exist

            foreach (var role in roles)
            {
                var hasRole = await _userManager.IsInRoleAsync(user, role);
                if (hasRole)
                    continue; // Skip if user already has this role

                var result = await _userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                    throw new Exception($"Failed to assign role '{role}' to user: {email}");
            }
        }
    }
}
