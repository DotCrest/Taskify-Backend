namespace Domain.Models;

public class VerificationCode
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsVerified { get; set; }
    public bool IsActive => DateTime.UtcNow < CreatedAt.AddMinutes(10);
}
