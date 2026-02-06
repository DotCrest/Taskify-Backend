using Application.Dtos;
using Application.Shared;

namespace Application.ServiceAbstractions;

public interface IAuthenticationService
{
    Task<Result<AuthResponseDto>> Login(LoginDto loginDto);
    Task<Result<AuthResponseDto>> Register(RegisterDto registerDto);
    Task<Result<AuthResponseDto>> GenerateNewTokenAsync(string refreshToken);
    Task<Result<BaseToReturnDto>> RevokeTokenAsync(string refreshToken);
    Task<Result<BaseToReturnDto>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, string email);
    Task<Result<BaseToReturnDto>> ForgetPasswordAsync(string email);
    Task<Result<BaseToReturnDto>> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto);
}
