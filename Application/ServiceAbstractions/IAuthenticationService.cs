using Application.Dtos;
using Application.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceAbstractions
{
    public interface IAuthenticationService
    {
        Task<Result<AuthResponseDto>> Login(LoginDto loginDto);
        Task<Result<AuthResponseDto>> Register(RegisterDto registerDto);
    }
}
