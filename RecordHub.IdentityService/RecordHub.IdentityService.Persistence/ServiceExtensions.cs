using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.IdentityService.Domain.Data.Entities;

namespace RecordHub.IdentityService.Persistence
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabaseContext(configuration);
            services.AddAspIdentity();
            services.Configure<IdentityOptions>(options => options.User.RequireUniqueEmail = true);
            return services;
        }
        private static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            return services.AddNpgsql<AccountDbContext>(connectionString);
        }

        private static void AddAspIdentity(this IServiceCollection services)
        {
            services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<AccountDbContext>()
                .AddRoles<IdentityRole<Guid>>();
        }
    }
}
