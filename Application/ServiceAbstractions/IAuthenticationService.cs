using Application.Dtos;
using Application.Shared;

namespace Application.ServiceAbstractions;

public interface IAuthenticationService
{
    Task<Result<AuthResponseDto>> Login(LoginDto loginDto);
    Task<Result<AuthResponseDto>> Register(RegisterDto registerDto);
    Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
    Task<Result<bool>> RevokeTokenAsync(string refreshToken);
}
