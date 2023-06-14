using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.IdentityService.Domain.Constants;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Persistence.Data.Repositories.Generic;
using RecordHub.IdentityService.Persistence.Data.Repositories.Implementation;

namespace RecordHub.IdentityService.Persistence
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabaseContext(configuration).AddRepositories();
            services.AddAspIdentity();
            services.Configure<IdentityOptions>(options => options.User.RequireUniqueEmail = true);
            return services;
        }
        private static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddNpgsql<AccountDbContext>(Constants.IdentityDbConnectionString);
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services.AddScoped<IUserRepository, UserRepository>();
        }

        private static void AddAspIdentity(this IServiceCollection services)
        {
            services.AddIdentityCore<User>()
                .AddRoles<IdentityRole<Guid>>()
                .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
                .AddRoleValidator<RoleValidator<IdentityRole<Guid>>>()
                .AddEntityFrameworkStores<AccountDbContext>();
        }
    }
}
