using Domain.Constants;
using Domain.Contracts;
using Domain.Models;
using Infrastructure.context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class DataSeeding(ApplicationDbContext _dbContext, RoleManager<IdentityRole> _roleManager
    , UserManager<User> _userManager) : IDataSeeding
{


    public async Task SeedDataAsync()
    {

        var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await _dbContext.Database.MigrateAsync();
        }
        if (!_roleManager.Roles.Any())
        {
            var roles = new[] { Role.Admin, Role.Member };
            foreach (var role in roles)
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var AdminUser = new User
        {
            UserName = "admin",
            Email = "admin@gmail.com",
            Name = "admin",
            JoinedAt = DateTime.UtcNow,
            EmailConfirmed = true

        };
        var user = new User()
        {
            UserName = "member",
            Email = "member@gmail.com",
            Name = "member",
            JoinedAt = DateTime.UtcNow,
            EmailConfirmed = true
        };
        await _userManager.CreateAsync(AdminUser, "Admin@123");
        await _userManager.CreateAsync(user, "User@123");
        await _userManager.AddToRoleAsync(AdminUser, Role.Admin);
        await _userManager.AddToRoleAsync(user, Role.Member);


    }
}
