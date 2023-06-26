using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.BasketService.Infrastructure.Data.Repositories;
using StackExchange.Redis;

namespace RecordHub.BasketService.Infrastructure
{
    public static class PersistenceServiceExtensions
    {
        public static IServiceCollection AddRedisPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                  ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnectionString")));
            services.AddScoped<IBasketRepository, BasketRepository>();

            return services;
        }
    }
}
