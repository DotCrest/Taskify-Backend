using Application.ServiceAbstractions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class AccountService(UserManager<User> userManager) : IAccountService
{
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user;
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user;
    }
}
