using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.OrderingService.Infrastructure.Data;
using RecordHub.OrderingService.Infrastructure.Data.Repositories;

namespace RecordHub.OrderingService.Infrastructure.Extensions
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabaseContext(configuration).AddRepositories();
            return services;
        }
        private static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddNpgsql<OrderingDbContext>(configuration.GetConnectionString("DefaultConnectionString"));
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services.AddScoped<IOrderingRepository, OrderingRepository>();
        }
    }
}
