using Domain.Common;
using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class FcmDeviceTokens : AuditableEntity 
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }          
        public string? DeviceToken { get; set; }    
        public string? DeviceType { get; set; }    
        public bool IsActive { get; set; } = true;
      
    }
}
