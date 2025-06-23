using Api.ApiResDto;
using Api.Dto;
using Application.DTOs;
using Application.ServiceContracts.Auth;
using Domain.Identity;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUser _currentUser;


        public AuthController(IAuthService authService, ICurrentUser currentUser)
        {
            _authService = authService;
            _currentUser = currentUser;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(false, "Invalid registration data.", ModelState));
            }
            var result = await _authService.RegisterAsync(registerDTO);
            if (result)
            {
                return Ok(new ApiResponse<object>(true, "Registration successful."));
            }
            return BadRequest(new ApiResponse<object>(false, "Registration failed"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(false, "Invalid login data.", ModelState));
            }
            var result = await _authService.LoginAsync(loginDTO);
            if (result != null)
            {
                return Ok(new ApiResponse<object>(true, "Login successful.", result));
            }
            return Unauthorized(new ApiResponse<object>(false, "Invalid credentials"));
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(false, "Invalid token data.", ModelState));
            }
            var result = await _authService.RefreshTokenAsync(tokenModel);
            if (result != null)
            {
                return Ok(new ApiResponse<object>(true, "Token refreshed successfully.", result));
            }
            return Unauthorized(new ApiResponse<object>(false, "Invalid token"));
        }

        [Authorize]
        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken([FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new ApiResponse<object>(false, "Token is required"));
            }
            var isValid = await _authService.ValidateToken(token);
            if (isValid)
            {
                return Ok(new ApiResponse<object>(true, "Token is valid"));
            }
            return Unauthorized(new ApiResponse<object>(false, "Invalid token"));
        }

        //logout
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new ApiResponse<object>(false, "Missing or invalid Authorization header."));
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var result = await _authService.RevokeToken(token);
            if (!result)
                return Unauthorized(new ApiResponse<object>(false, "Logout failed or token invalid."));

            return Ok(new ApiResponse<object>(true, "Successfully logged out."));
        }


        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {

            var userId = await _currentUser.GetCurrentUserIdAsync();
            if (userId == Guid.Empty)
            {
                return Unauthorized(new ApiResponse<object>(false, "User not found."));
            }
            var userDetails = await _currentUser.GetCurrentUserAsync();
            if (userDetails == null)
            {
                return NotFound(new ApiResponse<object>(false, "User details not found."));
            }

            var userDto = new UserDto
            {
                Id = userDetails.Id,
                UserName = userDetails.UserName,
                Email = userDetails.Email,
                PersonName = userDetails.PersonName,
                IsActive = userDetails.IsActive,
            };
            return Ok(new ApiResponse<object>(true, "User details retrieved successfully.", userDto));


        }
    }
}
