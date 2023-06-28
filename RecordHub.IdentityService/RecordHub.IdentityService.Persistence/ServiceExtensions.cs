using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.IdentityService.Core.TokenProviders;
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
            services.AddScoped<DbInitializer, DbInitializer>();
            services.Configure<IdentityOptions>(options => options.User.RequireUniqueEmail = true);
            return services;
        }
        private static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddNpgsql<AccountDbContext>(configuration.GetConnectionString("DefaultConnectionString"));
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAddressRepository, AddressRepository>();
            return services.AddScoped<IUserRepository, UserRepository>();
        }

        private static void AddAspIdentity(this IServiceCollection services)
        {
            services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequiredLength = 7;
                opt.Password.RequireDigit = false;
                opt.Password.RequireUppercase = false;
                opt.User.RequireUniqueEmail = true;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
            }
            )
                .AddRoles<IdentityRole<Guid>>()
                .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
                .AddRoleValidator<RoleValidator<IdentityRole<Guid>>>()
                .AddEntityFrameworkStores<AccountDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<EmailConfirmationTokenProvider<User>>("emailconfirmation");
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
    opt.TokenLifespan = TimeSpan.FromHours(2));
            services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromDays(3));
        }
    }
}
