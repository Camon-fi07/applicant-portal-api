using User.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace User.DAL
{
    public class UserDBContext : IdentityDbContext<UserEntity, Role, Guid, IdentityUserClaim<Guid>,
    IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<TokenEntity> BannedTokens { get; set; }


        public UserDBContext(DbContextOptions options) : base(options)
        {

        }
    }
}
