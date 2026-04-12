namespace Application.Dtos.AuthenticationDtos
{
    public class ResetPasswordDto : BasePasswordDto
    {

        public string OldPassword { get; set; } = null!;

    }
}
