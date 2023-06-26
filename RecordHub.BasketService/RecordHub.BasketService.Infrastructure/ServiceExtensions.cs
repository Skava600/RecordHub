using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.BasketService.Applicatation.Services;
using RecordHub.BasketService.Applicatation.Validators;
using RecordHub.BasketService.Infrastructure.Data.Repositories;
using RecordHub.Shared.Extensions;
using StackExchange.Redis;

namespace RecordHub.BasketService.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRedisPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                  ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnectionString")));
            services.AddScoped<IBasketRepository, BasketRepository>();

            return services;
        }

        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IBasketService, RecordHub.BasketService.Infrastructure.Services.BasketService>();
            services.AddValidatorsFromAssemblyContaining(typeof(ShoppingCartItemValidator));
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
