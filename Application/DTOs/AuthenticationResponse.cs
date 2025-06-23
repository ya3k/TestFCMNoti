using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AuthenticationResponse
    {
        public string? PersonName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public List<string>? Roles { get; set; }
        public string? AccessToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpirationDateTime { get; set; }
    }
}
