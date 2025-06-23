using Microsoft.AspNetCore.Identity;

namespace Domain.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? PersonName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int StreakDays { get; set; } // Số ngày học liên tiếp
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

      
    }
}
