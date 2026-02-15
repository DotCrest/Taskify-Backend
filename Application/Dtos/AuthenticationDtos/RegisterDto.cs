namespace Application.Dtos.AuthenticationDtos;

public class RegisterDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}
