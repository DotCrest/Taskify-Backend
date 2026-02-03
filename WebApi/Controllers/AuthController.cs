using Application.Dtos;
using Application.ServiceAbstractions;
using Application.Shared.Errors;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthenticationService _authenticationService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromForm] LoginDto loginDto)
    {
        var authResponse = await _authenticationService.Login(loginDto);


        return authResponse.Map<ActionResult<AuthResponseDto>>(
             onSuccess: result =>
             {
                 SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
                 return Ok(result);
             },
                onFailure: error => BadRequest(error)
        );

    }
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromForm] RegisterDto registerDto)
    {
        var authResponse = await _authenticationService.Register(registerDto);
        return authResponse.Map<ActionResult<AuthResponseDto>>(
              onSuccess: result =>
              {

                  SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
                  return Ok(result);
              },
              onFailure: error => BadRequest(error)
        );
    }
    [HttpGet("refreshToken")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var authResponse = await _authenticationService.GenerateNewTokenAsync(refreshToken!);
        return authResponse.Map<ActionResult<AuthResponseDto>>(
              onSuccess: result =>
              {
                  if (!string.IsNullOrEmpty(result.RefreshToken))
                  {

                      SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
                  }
                  return Ok(result);
              },
              onFailure: error => BadRequest(error)
        );
    }
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenDto revokeTokenDto)
    {
        var refreshToken = revokeTokenDto.Token ?? Request.Cookies["refreshToken"];
        if (string.IsNullOrWhiteSpace(refreshToken))
            return BadRequest(AuthErrors.InvalidRefreshToken);

        var result = await _authenticationService.RevokeTokenAsync(refreshToken!);

        return result.Map<IActionResult>(
            _ => NoContent(),
            error => BadRequest(error)
        );
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        var result = await _authenticationService.ResetPasswordAsync(email: resetPasswordDto.Email, newPassword: resetPasswordDto.NewPassword, token: resetPasswordDto.Token);
        return result.Map<IActionResult>(
            onSuccess: _ => NoContent(),
            onFailure: error => BadRequest(error)
            );
    }




    private void SetRefreshTokenInCookie(string refreshToken, DateTime expiresOn)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiresOn.ToLocalTime()
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

    }


    //TODO: Forgot Password Endpoint


}
