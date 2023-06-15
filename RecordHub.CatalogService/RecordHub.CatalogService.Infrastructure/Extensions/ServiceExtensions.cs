using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.CatalogService.Application.Services;
using RecordHub.CatalogService.Infrastructure.Services;
using RecordHub.Shared.Extensions;
using System.Reflection;

namespace RecordHub.CatalogService.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IRecordCatalogService, RecordCatalogService>();
            return services;
        }

        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfiguredJwtBearer(configuration);
            services.AddAuthorization();

            return services;
        }

    }
}


