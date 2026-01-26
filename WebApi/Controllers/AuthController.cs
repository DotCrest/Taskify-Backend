using Application.Dtos;
using Application.ServiceAbstractions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthenticationService _authenticationService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromForm] LoginDto loginDto)
        {
            var authResponse = await _authenticationService.Login(loginDto);
            return authResponse.Map<ActionResult<AuthResponseDto>>(
                 onSuccess: result => Ok(result),
                 onFailure: error => BadRequest(error)
            );
        }
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromForm] RegisterDto registerDto)
        {
            var authResponse = await _authenticationService.Register(registerDto);
            return authResponse.Map<ActionResult<AuthResponseDto>>(
                  onSuccess: result => Ok(result),
                  onFailure: error => BadRequest(error)
            );
        }
       
        // TODO: Refresh Token Endpoint
        //TODO: reset Password Endpoint
        //TODO: Forgot Password Endpoint


    }
}
