using Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace User.DAL.Entities
{
    public class Role:IdentityRole<Guid>
    {
        public RoleType RoleType { get; set; }
    }
}
