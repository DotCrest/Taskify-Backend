using Application.Dtos;
using Application.Shared;

namespace Application.ServiceAbstractions;

public interface ICodeVerificationService
{
    Task<BaseToReturnDto> SendCode(string email);
    Task<Result<BaseToReturnDto>> ValidateCode(VerifyCodeDto verifyCodeDto);
    Task<bool> IsValidated(string email);
}
