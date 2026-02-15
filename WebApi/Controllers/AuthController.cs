using Application.Common.Errors;
using Application.Dtos;
using Application.Dtos.AuthenticationDtos;
using Application.ServiceAbstractions;
using Application.Shared.Errors;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class AuthController(IAuthenticationService authenticationService,
                      IValidator<LoginDto> loginDtoValidator,
                      IValidator<RegisterDto> registerDtoValidator,
                      IValidator<ResetPasswordDto> resetPasswordDtoValidator,
                      IValidator<UpdatePasswordDto> updatePasswordDtoValidator) : BaseApiController
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromForm] LoginDto loginDto)
    {
        // validation 
        var validation = await ExecuteWithValidation(loginDtoValidator, loginDto);
        if (!validation.IsSuccess)
            return HandleFailure(validation.ErrorsList);
        // business logic
        var authResponse = await authenticationService.Login(loginDto);
        return authResponse.Map<ActionResult<AuthResponseDto>>(
            onSuccess: result =>
            {
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
                return Ok(result);
            },
            onFailure: error => HandleFailure(error)
        );
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromForm] RegisterDto registerDto)
    {
        // validation
        var validation = await ExecuteWithValidation(registerDtoValidator, registerDto);
        if (!validation.IsSuccess)
            return HandleFailure(validation.ErrorsList);
        // business logic
        var authResponse = await authenticationService.Register(registerDto);
        return authResponse.Map<ActionResult<AuthResponseDto>>(
            onSuccess: result =>
            {

                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
                return Ok(result);
            },
            onFailure: error => HandleFailure(error)
        );
    }

    [HttpGet("refreshToken")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var authResponse = await authenticationService.GenerateNewTokenAsync(refreshToken!);
        return authResponse.Map<ActionResult<AuthResponseDto>>(
              onSuccess: result =>
              {
                  if (!string.IsNullOrEmpty(result.RefreshToken))
                  {

                      SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
                  }
                  return Ok(result);
              },
              onFailure: error => HandleFailure(error)
        );
    }

    [HttpPost("revoke")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseToReturnDto>> Revoke([FromBody] RevokeTokenDto revokeTokenDto)
    {
        var refreshToken = revokeTokenDto.Token ?? Request.Cookies["refreshToken"];
        if (string.IsNullOrWhiteSpace(refreshToken))
            return BadRequest(AuthErrors.InvalidRefreshToken);

        var result = await authenticationService.RevokeTokenAsync(refreshToken!);

        return result.Map<ActionResult<BaseToReturnDto>>(
            data => Ok(data),
            error => HandleFailure(error)
        );
    }

    [Authorize]
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(BaseToReturnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseToReturnDto>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        // validation
        var validation = await ExecuteWithValidation(resetPasswordDtoValidator, resetPasswordDto);
        if (!validation.IsSuccess)
            return HandleFailure(validation.ErrorsList);
        // business logic
        var email = User.FindFirstValue(ClaimTypes.Email);
        var result = await authenticationService.ResetPasswordAsync(resetPasswordDto, email);
        return result.Map<ActionResult<BaseToReturnDto>>(
            onSuccess: data => Ok(data),
            onFailure: error => HandleFailure(error)
            );
    }

    [HttpPost("forget-password")]
    [ProducesResponseType(typeof(BaseToReturnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseToReturnDto>> ForgetPassword([FromBody] ForgetPasswordDto forgetPasswordDto)
    {
        var result = await authenticationService.ForgetPasswordAsync(forgetPasswordDto);
        return result.Map<ActionResult<BaseToReturnDto>>(
            onSuccess: data => Ok(data),
            onFailure: error => HandleFailure(error)
        );
    }

    [HttpPost("update-password")]
    [ProducesResponseType(typeof(BaseToReturnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseToReturnDto>> UpdatePassword([FromBody] UpdatePasswordDto updatePasswordDto)
    {
        // validation
        var validation = await ExecuteWithValidation(updatePasswordDtoValidator, updatePasswordDto);
        if (!validation.IsSuccess)
            return HandleFailure(validation.ErrorsList);
        // business logic
        var result = await authenticationService.UpdatePasswordAsync(updatePasswordDto);
        return result.Map<ActionResult<BaseToReturnDto>>(
            onSuccess: data => Ok(data),
            onFailure: error => HandleFailure(error)
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




}
