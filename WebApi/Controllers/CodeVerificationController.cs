using Application.Dtos;
using Application.ServiceAbstractions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CodeVerificationController(ICodeVerificationService verificationService) : ControllerBase
    {
        // validate code endpoint
        [HttpPost("validate-code")]
        public async Task<ActionResult<BaseToReturnDto>> ValidateCode([FromForm] VerifyCodeDto verifyCodeDto)
        {
            var result = await verificationService.ValidateCode(verifyCodeDto);
            return result.Map<ActionResult<BaseToReturnDto>>(
                onSuccess: res => Ok(res),
                onFailure: err => BadRequest(err)
            );
        }
    }
}
