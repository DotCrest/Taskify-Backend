using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.DataSeeding;

public class UserSeeder
{
    private readonly UserManager<User> _userManager;

    public UserSeeder(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        // Check if users already exist
        var existingAdmin = await _userManager.FindByEmailAsync("admin@taskify.com");
        if (existingAdmin != null)
            return;

        // Create seed users
        var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin",
                Email = "admin@taskify.com",
                EmailConfirmed = true,
                Name = "Administrator",
                PhotoUrl = null,
                JoinedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "john_doe",
                Email = "john@taskify.com",
                EmailConfirmed = true,
                Name = "John Doe",
                PhotoUrl = null,
                JoinedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "jane_smith",
                Email = "jane@taskify.com",
                EmailConfirmed = true,
                Name = "Jane Smith",
                PhotoUrl = null,
                JoinedAt = DateTime.UtcNow
            }
        };

        foreach (var user in users)
        {
            var result = await _userManager.CreateAsync(user, "Password@123");
            if (!result.Succeeded)
                throw new Exception($"Failed to seed user: {user.Email}");
        }
    }
}
