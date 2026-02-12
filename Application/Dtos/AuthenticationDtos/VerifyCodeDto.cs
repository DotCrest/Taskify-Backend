namespace Application.Dtos.AuthenticationDtos;

public class VerifyCodeDto
{
    public string Code { get; set; } = null!;
    public string Email { get; set; } = null!;
}
