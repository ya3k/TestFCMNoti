using Application.DTOs;
using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceContracts
{
    public interface IJwtService
    {

        Task<AuthenticationResponse> CreateJwtToken(ApplicationUser user);
        Task<ClaimsPrincipal?> GetPrincipalFromJwtToken(string? token);
    }
}
