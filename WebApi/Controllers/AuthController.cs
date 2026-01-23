using Application.Dtos;
using Application.ServiceAbstractions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthenticationService _authenticationService):ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromForm] LoginDto loginDto)
        {
            var authResponse = await _authenticationService.Login(loginDto);
            if (!authResponse.IsAuthenticated)
            {
                return Unauthorized(authResponse);
            }
            return Ok(authResponse);
        }
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromForm] RegisterDto registerDto)
        {
            var authResponse = await _authenticationService.Register(registerDto);
            if (!authResponse.IsAuthenticated)
            {
                return BadRequest(authResponse);
            }
            return Ok(authResponse);
        }
    }
}
