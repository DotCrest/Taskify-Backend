using Application.Common.Errors;
using Application.Dtos;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
using Domain;
using Domain.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthenticationService(UserManager<User> _userManager, IOptions<JwtOptions> _options) : IAuthenticationService
    {
        public async Task<Result<AuthResponseDto>> Login(LoginDto loginDto)
        {


            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                //return new AuthResponseDto()
                //{
                //    Message = "Invalid Email Or Password"
                //    ,
                //    IsAuthenticated = false
                //};
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
            authResponse.Token = jwtToken;
            authResponse.ExpiresOn = DateTime.UtcNow.AddDays(30);
            authResponse.Username = user.UserName;
            authResponse.Email = user.Email;
            authResponse.Roles = roles.ToList();

            return Result<AuthResponseDto>.Success(authResponse);

        }

        public async Task<Result<AuthResponseDto>> Register(RegisterDto registerDto)
        {

            if (await _userManager.FindByEmailAsync(registerDto.Email) is not null)
            {

                return Result<AuthResponseDto>.Failure(AuthErrors.EmailAlreadyExsists);

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
                    .Failure(result.Errors.Select(x => new Error(x.Code,x.Description)).ToList());
            }
            var roles = await _userManager.AddToRoleAsync(user, "User");
            var jwtToken = await CreateTokenAsync(user);
            var authRespons = new AuthResponseDto()
            {

                IsAuthenticated = true,
                Token = jwtToken,
                ExpiresOn = DateTime.UtcNow.AddDays(30),
                Username = user.UserName,
                Email = user.Email,
                Message = "User Registered Successfully",
                Roles = new List<string>() { "User" }
            };
            return Result<AuthResponseDto>.Success(authRespons);


        }
        private async Task<string> CreateTokenAsync(User user)
        {
            var JwtOptions = _options.Value;
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.Email,user.Email)
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
                expires: DateTime.UtcNow.AddDays(30));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
      
    }
}
