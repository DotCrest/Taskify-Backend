namespace Application.Dtos.AuthenticationDtos
{
    public class BasePasswordDto
    {
        public string NewPassword { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
