using Application.Common.Errors;
using Application.Dtos;
using Application.Dtos.AuthenticationDtos;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
using Domain.Models;
using Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace Application.Services;

public class AuthenticationService(UserManager<User> _userManager, IOptions<JwtOptions> _options,
                                   PasswordHasher<User> passwordHasher, ICodeVerificationService _codeVerificationService) : IAuthenticationService
{
    public async Task<Result<AuthResponseDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {

            return Result<AuthResponseDto>.Failure(AuthErrors.InvalidCredentials);


        }
        if (await _userManager.IsLockedOutAsync(user))
        {
            return Result<AuthResponseDto>.Failure(AuthErrors.UserLockedOut);

        }

        var roles = await _userManager.GetRolesAsync(user);
        var jwtToken = await CreateTokenAsync(user);
        var authResponse = new AuthResponseDto();
        authResponse.IsAuthenticated = true;
        authResponse.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        authResponse.ExpiresOn = jwtToken.ValidTo;
        authResponse.Username = user.UserName;
        authResponse.Email = user.Email;
        authResponse.Roles = roles.ToList();
        if (user.RefeshTokens.Any(t => t.IsActive))
        {
            var activeRefreshToken = user.RefeshTokens.FirstOrDefault(t => t.IsActive);
            authResponse.RefreshToken = activeRefreshToken!.Token;
            authResponse.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
        }
        else
        {
            var refreshToken = CreateRefreshToken();
            user.RefeshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
            authResponse.RefreshToken = refreshToken.Token;
            authResponse.RefreshTokenExpiration = refreshToken.ExpiresOn;
        }

        return Result<AuthResponseDto>.Success(authResponse);

    }

    public async Task<Result<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        if (await _userManager.FindByEmailAsync(registerDto.Email) is not null)
        {

            return Result<AuthResponseDto>.Failure(AuthErrors.EmailAlreadyExists);

        }
        if (await _userManager.FindByNameAsync(registerDto.UserName) is not null)
        {
            return Result<AuthResponseDto>.Failure(AuthErrors.UsernameAlreadyExsists);


        }

        var user = new User()
        {
            Email = registerDto.Email,
            UserName = registerDto.UserName,
            Name = registerDto.Name,
            JoinedAt = DateTime.UtcNow,
            EmailConfirmed = true
        };
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {

            return Result<AuthResponseDto>
                .Failure(result.Errors.Select(x => new Error(x.Code, x.Description)).ToList());
        }
        var roles = await _userManager.AddToRoleAsync(user, registerDto.Role);
        var jwtToken = await CreateTokenAsync(user);
        var refreshToken = CreateRefreshToken();
        user.RefeshTokens.Add(refreshToken);
        await _userManager.UpdateAsync(user);
        var authRespons = new AuthResponseDto()
        {

            IsAuthenticated = true,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            ExpiresOn = jwtToken.ValidTo,
            Username = user.UserName,
            Email = user.Email,
            Message = "User Registered Successfully",
            Roles = new List<string>() { registerDto.Role },
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.ExpiresOn
        };
        return Result<AuthResponseDto>.Success(authRespons);


    }
    private async Task<JwtSecurityToken> CreateTokenAsync(User user)
    {
        var JwtOptions = _options.Value;
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name,user.Name),
            new Claim(ClaimTypes.Email,user.Email),
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
        };
        var roloes = await _userManager.GetRolesAsync(user);
        foreach (var role in roloes)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SecretKey));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer: JwtOptions.Issuer,
            audience: JwtOptions.Audience,
            claims: claims,
            signingCredentials: signinCredentials,
            expires: DateTime.UtcNow.AddHours(JwtOptions.ExpirationInHours));
        return token;
    }
    public async Task<Result<AuthResponseDto>> GenerateNewTokenAsync(string refreshToken)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefeshTokens.Any(t => t.Token == refreshToken));
        if (user == null)
            return Result<AuthResponseDto>.Failure(AuthErrors.InvalidRefreshToken);
        var refreshtoken = user.RefeshTokens.SingleOrDefault(t => t.Token == refreshToken);
        if (!refreshtoken.IsActive)
            return Result<AuthResponseDto>.Failure(AuthErrors.InvalidRefreshToken);
        refreshtoken.RevokedOn = DateTime.UtcNow;
        var newRefreshToken = CreateRefreshToken();
        user.RefeshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);
        var jwtToken = await CreateTokenAsync(user);
        var authResponse = new AuthResponseDto()
        {
            IsAuthenticated = true,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            ExpiresOn = jwtToken.ValidTo,
            Username = user.UserName,
            Email = user.Email,
            RefreshToken = newRefreshToken.Token,
            Roles = (await _userManager.GetRolesAsync(user)).ToList(),
        };
        return Result<AuthResponseDto>.Success(authResponse);


    }

    public async Task<Result<BaseToReturnDto>> RevokeTokenAsync(string refreshToken)
    {


        var user = await _userManager.Users
            .SingleOrDefaultAsync(u =>
                u.RefeshTokens.Any(t => t.Token == refreshToken));

        if (user is null)
            return Result<BaseToReturnDto>.Failure(AuthErrors.InvalidRefreshToken);

        var token = user.RefeshTokens
            .Single(t => t.Token == refreshToken);

        if (!token.IsActive)
            return Result<BaseToReturnDto>.Failure(AuthErrors.InvalidRefreshToken);

        token.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
        var baseToReturnDto = new BaseToReturnDto()
        {
            IsSuccess = true,
            Message = "Refresh token revoked successfully."
        };

        return Result<BaseToReturnDto>.Success(baseToReturnDto);
    }
    private RefeshToken CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var generator = new RNGCryptoServiceProvider();
        generator.GetBytes(randomNumber);
        var refreshToken = new RefeshToken()
        {
            Token = Convert.ToBase64String(randomNumber),
            ExpiresOn = DateTime.UtcNow.AddDays(7),
            CreatedOn = DateTime.UtcNow
        };
        return refreshToken;

    }

    public async Task<Result<BaseToReturnDto>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var IsPasswordCorrect = await _userManager.CheckPasswordAsync(user, resetPasswordDto.OldPassword);
        if (!IsPasswordCorrect)
        {
            return Result<BaseToReturnDto>.Failure(new Error("InvalidOldPassword", "The old password is incorrect."));
        }
        var PasswordHash = passwordHasher.HashPassword(user, resetPasswordDto.NewPassword);
        user.PasswordHash = PasswordHash;
        await _userManager.UpdateAsync(user);
        var baseToReturnDto = new BaseToReturnDto()
        {
            IsSuccess = true,
            Message = "Password has been reset successfully."
        };
        return Result<BaseToReturnDto>.Success(baseToReturnDto);

    }

    public async Task<Result<BaseToReturnDto>> ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(forgetPasswordDto.Email);
        if (user is null)
            return Result<BaseToReturnDto>.Failure(AuthErrors.UserNotFound);
        var baseToReturnDto = await _codeVerificationService.SendCode(forgetPasswordDto.Email);
        return Result<BaseToReturnDto>.Success(baseToReturnDto);
    }
    public async Task<Result<BaseToReturnDto>> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(updatePasswordDto.Email);
        if (user is null)
            return Result<BaseToReturnDto>.Failure(AuthErrors.UserNotFound);
        user.PasswordHash = passwordHasher.HashPassword(user, updatePasswordDto.NewPassword);
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return Result<BaseToReturnDto>
                .Failure(result.Errors.Select(x => new Error(x.Code, x.Description)).ToList());
        }
        var baseToReturnDto = new BaseToReturnDto()
        {
            IsSuccess = true,
            Message = "Password has been updated successfully."
        };
        return Result<BaseToReturnDto>.Success(baseToReturnDto);

    }


}
