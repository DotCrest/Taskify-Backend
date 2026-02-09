using Application.Common.Errors;
using Application.Dtos;
using Application.ServiceAbstractions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CodeVerificationController(ICodeVerificationService verificationService) : BaseApiController
    {
        // validate code endpoint
        [HttpPost("validate-code")]
        [ProducesResponseType(typeof(BaseToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseToReturnDto>> ValidateCode([FromForm] VerifyCodeDto verifyCodeDto)
        {
            var result = await verificationService.ValidateCode(verifyCodeDto);
            return result.Map<ActionResult<BaseToReturnDto>>(
                onSuccess: res => Ok(res),
                onFailure: err => HandleFailure(err)
            );
        }
    }
}
