using Domain.Models;
using Infrastructure.context;

namespace Infrastructure.DataSeeding;

public class VerificationCodeSeeder
{
    private readonly ApplicationDbContext _context;

    public VerificationCodeSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Check if verification codes already exist
        if (_context.VerificationCodes.Any())
            return;

        var verificationCodes = new List<VerificationCode>
        {
            new VerificationCode
            {
                Code = "123456",
                Email = "test@taskify.com",
                CreatedAt = DateTime.UtcNow
            },
            new VerificationCode
            {
                Code = "654321",
                Email = "verify@taskify.com",
                CreatedAt = DateTime.UtcNow.AddMinutes(-5)
            }
        };

        await _context.VerificationCodes.AddRangeAsync(verificationCodes);
        await _context.SaveChangesAsync();
    }
}
