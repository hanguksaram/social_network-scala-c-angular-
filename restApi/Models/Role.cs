using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Models
{
    public class Role: IdentityRole<int>
    {
        public ICollection<UserRole> UserRoles { get; set; }
        
        public enum RoleTypes{ Member, Admin, Moderator, VIP }
    }
}
