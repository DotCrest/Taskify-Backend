namespace Domain.Models;

public class VerificationCode
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsActive => DateTime.Now < CreatedAt.AddMinutes(10);
}
