using Domain.Identity;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CurrentUserService : ICurrentUser
    {
        private ApplicationUser _currentUser;
        private UserManager<ApplicationUser> _userManager;
        private HttpContext _httpContext;

        public CurrentUserService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContext = httpContextAccessor.HttpContext;
            if (_httpContext != null && _httpContext.User.Identity.IsAuthenticated)
            {
                var userId = _httpContext.User.FindFirst("id")?.Value;
                if (userId != null)
                {
                    _currentUser = _userManager.FindByIdAsync(userId).Result;
                }
            }
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            if (_currentUser != null)
            {
                return _currentUser;
            }
            _currentUser = await _userManager.GetUserAsync(_httpContext.User);
            return _currentUser;
        }

        public async Task<Guid> GetCurrentUserIdAsync()
        {
            var user = await GetCurrentUserAsync();
            return user != null ? user.Id : Guid.Empty;
        }
    }
}
