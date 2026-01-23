using Application.Dtos;
using Application.ServiceAbstractions;
using Domain;
using Domain.Entites;
using Microsoft.AspNetCore.Identity;
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
        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {


            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return new AuthResponseDto()
                {
                    Message = "Invalid Email Or Password"
                    ,
                    IsAuthenticated = false
                };

            }
            if (await _userManager.IsLockedOutAsync(user))
            {
                return new AuthResponseDto()
                {
                    Message = "Account Is Locked duo to many failed to many attemps",
                    IsAuthenticated = false
                };

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

            return authResponse;

        }

        public async Task<AuthResponseDto> Register(RegisterDto registerDto)
        {
            //var authResponse = new AuthResponseDto();
            if (await _userManager.FindByEmailAsync(registerDto.Email) is not null)
            {
                return new AuthResponseDto()
                {
                    Message = "Email is already in use please chose another email",
                    IsAuthenticated = false
                };


            }
            if (await _userManager.FindByNameAsync(registerDto.UserName) is not null)
            {
                return new AuthResponseDto()
                {
                    Message = "Username is already in use please chose another username",
                    IsAuthenticated = false
                };

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
                //Some exception
            }
            var roles = await _userManager.AddToRoleAsync(user,"User");
            var jwtToken = await CreateTokenAsync(user);
            return new AuthResponseDto()
            {
                IsAuthenticated = true,
                Token = jwtToken,
                ExpiresOn = DateTime.UtcNow.AddDays(30),
                Username = user.UserName,
                Email = user.Email,
                Message = "User Registered Successfully",
                Roles = new List<string>() { "User" }
            };
        
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
            //d52ccc5e735ce59f56192e4a06895dfcf35dff6afb44fb56057d510f4d6b4030
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
