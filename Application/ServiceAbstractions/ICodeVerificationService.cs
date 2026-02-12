using Application.Dtos.AuthenticationDtos;
using Application.Shared;

namespace Application.ServiceAbstractions;

public interface ICodeVerificationService
{
    Task<BaseToReturnDto> SendCode(string email);
    Task<Result<BaseToReturnDto>> ValidateCode(VerifyCodeDto verifyCodeDto);
}
