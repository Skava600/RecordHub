using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Infrastructure.Data;

namespace RecordHub.CatalogService.Infrastructure.Extensions
{
    public static class PersistenceServiceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDatabaseContext(configuration)
                .AddRepositories();

            return services;
        }

        private static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddNpgsql<ApplicationDbContext>(configuration.GetConnectionString("DefaultConnectionString"));
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
