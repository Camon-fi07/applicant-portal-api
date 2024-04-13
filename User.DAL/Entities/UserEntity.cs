using Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace User.DAL.Entities
{
    public class UserEntity:IdentityUser<Guid>
    {
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public string Firstname { get; set; }
        public DateOnly BirthDate { get; set; }
        public Gender Gender { get; set; }
    }
}
