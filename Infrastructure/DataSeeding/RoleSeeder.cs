using Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.DataSeeding;

/// <summary>
/// Seeder class responsible for seeding application roles.
/// Creates standard roles (Admin, Member) used throughout the application.
/// </summary>
public class RoleSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleSeeder(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    /// <summary>
    /// Seeds the predefined application roles into the database.
    /// Only creates roles if they don't already exist.
    /// </summary>
    public async Task SeedAsync()
    {
        var roles = new[] { Role.Admin, Role.Member };

        foreach (var roleName in roles)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
                continue;

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
                throw new Exception($"Failed to seed role: {roleName}");
        }
    }
}
