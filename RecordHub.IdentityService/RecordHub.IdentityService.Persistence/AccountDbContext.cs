using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecordHub.IdentityService.Domain.Data.Entities;

namespace RecordHub.IdentityService.Persistence
{
    public class AccountDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {

        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(AccountDbContext).Assembly);
        }
    }
}
