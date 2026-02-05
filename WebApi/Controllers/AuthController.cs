using Application.Common.Errors;
using Application.Dtos;
using Application.ServiceAbstractions;
using Application.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


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
        return authResponse.MapList<ActionResult<AuthResponseDto>>(
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
    public async Task<ActionResult<BaseToReturnDto>> Revoke([FromBody] RevokeTokenDto revokeTokenDto)
    {
        var refreshToken = revokeTokenDto.Token ?? Request.Cookies["refreshToken"];
        if (string.IsNullOrWhiteSpace(refreshToken))
            return BadRequest(AuthErrors.InvalidRefreshToken);

        var result = await _authenticationService.RevokeTokenAsync(refreshToken!);

        return result.Map<ActionResult<BaseToReturnDto>>(
            _ => Ok(result),
            error => BadRequest(error)
        );
    }
    [Authorize]
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var result = await _authenticationService.ResetPasswordAsync(resetPasswordDto, email);
        return result.Map<ActionResult<bool>>(
            onSuccess: _ => Ok(result),
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
