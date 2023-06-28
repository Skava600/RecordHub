using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Enum;

namespace RecordHub.IdentityService.Persistence
{
    public class DbInitializer
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly AccountDbContext ctx;
        public DbInitializer(RoleManager<IdentityRole<Guid>> roleManager, UserManager<User> userManager, AccountDbContext dbContext)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            ctx = dbContext;
        }

        public async Task Initialize()
        {
            var pendingMigrations = await ctx.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                await ctx.Database.MigrateAsync();
            }
            var user = new User
            {
                Email = "uskava7@gmail.com",
                Name = "admin",
                Surname = "admin",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                EmailConfirmed = true,
                UserName = "admin",
                PhoneNumber = "123",
            };

            if (!ctx.Users.Any(u => u.UserName.Equals(user.UserName)))
            {
                var result = await _userManager.CreateAsync(user, "123456aA.");
                await _userManager.AddToRoleAsync(user, nameof(Roles.Admin));
            }


        }
    }
}
