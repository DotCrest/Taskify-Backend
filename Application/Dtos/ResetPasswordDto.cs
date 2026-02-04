namespace Application.Dtos
{
    public class ResetPasswordDto : BasePasswordDto
    {

        public string OldPassword { get; set; } = null!;

    }
}
