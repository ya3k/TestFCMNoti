using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class FcmNotification : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? ClickAction { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime ScheduledTime { get; set; } = DateTime.UtcNow;
    }
}
