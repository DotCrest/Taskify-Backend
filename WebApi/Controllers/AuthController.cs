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
                onFailure: error => HandelFailure(error)
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
              onFailure: error => HandelFailure(error)
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
              onFailure: error => HandelFailure(error)
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
            error => HandelFailure(error)
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
            onFailure: error => HandelFailure(error)
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
    [HttpPost("forget-password")]
    public async Task<ActionResult<BaseToReturnDto>> ForgetPassword([FromBody] string email)
    {
        var result = await _authenticationService.ForgetPasswordAsync(email);
        return result.Map<ActionResult<BaseToReturnDto>>(
            onSuccess: _ => Ok(result),
            onFailure: error => HandelFailure(error)
        );
    }
    [HttpPost("update-password")]
    public async Task<ActionResult<BaseToReturnDto>> UpdatePassword([FromBody] UpdatePasswordDto updatePasswordDto)
    {
        var result = await _authenticationService.UpdatePasswordAsync(updatePasswordDto);
        return result.MapList<ActionResult<BaseToReturnDto>>(
            onSuccess: _ => Ok(result),
            onFailure: error => HandelFailure(error)
        );
    }
    private ActionResult HandelFailure(object errors)
    {
        var errorCode = errors switch
        {
            Error e => e.Code,
            IEnumerable<Error> es => es.FirstOrDefault()?.Code,
            _ => string.Empty
        };

        return errorCode switch
        {
            var code when code.Contains("NotFound") => NotFound(errors),
            var code when code.Contains("Unauthorized") => Unauthorized(errors),
            _ => BadRequest(errors)
        };

    }




}
