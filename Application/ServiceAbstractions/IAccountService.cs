using Domain.Models;

namespace Application.ServiceAbstractions;

public interface IAccountService
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(string userId);
}