using Application.DTOs;
using Application.ServiceContracts;
using Application.ServiceContracts.Auth;
using Domain.Identity;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ICurrentUser _currentUser;
        private const string TOKEN_PROVIDER = "MyApp";
        private const string REFRESH_TOKEN_NAME = "RefreshToken";
        private const string REFRESH_TOKEN_EXP_NAME = "RefreshTokenExpiration";

        public AuthService(UserManager<ApplicationUser> userManager, IJwtService jwtService, ICurrentUser currentUser)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _currentUser = currentUser;
        }

        public async Task<bool> RegisterAsync(RegisterDTO registerDTO)
        {
            var user = new ApplicationUser
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                PersonName = registerDTO.PersonName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, ApplicationRole.USER);
                return true;
            }

            return false;
        }

        public async Task<AuthenticationResponse> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var tokens = await _jwtService.CreateJwtToken(user);

            await _userManager.SetAuthenticationTokenAsync(user, TOKEN_PROVIDER, REFRESH_TOKEN_NAME, tokens.RefreshToken);
            await _userManager.SetAuthenticationTokenAsync(user, TOKEN_PROVIDER, REFRESH_TOKEN_EXP_NAME, tokens.RefreshTokenExpirationDateTime.ToString("O")); // ISO 8601

            return tokens;
        }


        public async Task<AuthenticationResponse> RefreshTokenAsync(TokenModel tokenModel)
        {
            var userId = await _currentUser.GetCurrentUserIdAsync();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var savedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, TOKEN_PROVIDER, REFRESH_TOKEN_NAME);
            var savedExpString = await _userManager.GetAuthenticationTokenAsync(user, TOKEN_PROVIDER, REFRESH_TOKEN_EXP_NAME);

            if (savedRefreshToken != tokenModel.RefreshToken)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            if (!DateTime.TryParse(savedExpString, out var refreshExp) || refreshExp <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token expired.");

            var newTokens = await _jwtService.CreateJwtToken(user);

            await _userManager.SetAuthenticationTokenAsync(user, TOKEN_PROVIDER, REFRESH_TOKEN_NAME, newTokens.RefreshToken);
            await _userManager.SetAuthenticationTokenAsync(user, TOKEN_PROVIDER, REFRESH_TOKEN_EXP_NAME, newTokens.RefreshTokenExpirationDateTime.ToString("O"));

            return newTokens;
        }

        public async Task<bool> ValidateToken(string token)
        {
            var principal = await _jwtService.GetPrincipalFromJwtToken(token);
            return principal != null;
        }

        public async Task<bool> RevokeToken(string token)
        {
            var principal = await _jwtService.GetPrincipalFromJwtToken(token);
            if (principal == null) return false;

            var email = principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return false;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            await _userManager.RemoveAuthenticationTokenAsync(user, TOKEN_PROVIDER, REFRESH_TOKEN_NAME);
            await _userManager.RemoveAuthenticationTokenAsync(user, TOKEN_PROVIDER, REFRESH_TOKEN_EXP_NAME);

            return true;
        }
    }
}
