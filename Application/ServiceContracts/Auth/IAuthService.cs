using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceContracts.Auth
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDTO registerDTO);
        Task<AuthenticationResponse> LoginAsync(LoginDTO loginDTO);
        Task<AuthenticationResponse> RefreshTokenAsync(TokenModel tokenModel);
        Task<bool> ValidateToken(string token);
        Task<bool> RevokeToken(string token);
    

    }
}
