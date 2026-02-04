using Application.Dtos;
using Application.ServiceAbstractions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CodeVerificationController(ICodeVerificationService verificationService) : ControllerBase
    {
        // send code endpoint
        [HttpPost("send-code")]
        public async Task<ActionResult<BaseToReturnDto>> SendCode([FromForm] string email)
        {
            var result = await verificationService.SendCode(email);
            return Ok(result);
        }
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
