using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Identity
{
    public class ApplicationRole: IdentityRole<Guid>
    {
        public const string ADMIN = "Admin";
        public const string USER = "User"; 
        public const string VIPMEMBER = "VipMember";


    }
}
