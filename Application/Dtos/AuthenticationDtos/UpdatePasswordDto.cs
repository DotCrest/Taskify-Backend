namespace Application.Dtos.AuthenticationDtos
{
    public class UpdatePasswordDto
    {
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
