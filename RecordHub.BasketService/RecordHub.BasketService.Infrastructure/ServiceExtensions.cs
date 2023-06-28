using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RecordHub.BasketService.Applicatation.Services;
using RecordHub.BasketService.Applicatation.Validators;
using RecordHub.BasketService.Infrastructure.Config;
using RecordHub.BasketService.Infrastructure.Data.Repositories;
using RecordHub.Shared.Extensions;
using StackExchange.Redis;

namespace RecordHub.BasketService.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRedisPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisConfig>(configuration.GetSection("Redis"));
            services.AddSingleton<RedisConfig>(sp => sp.GetRequiredService<IOptions<RedisConfig>>().Value);
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var multConfig = ConfigurationOptions.Parse(configuration.GetConnectionString("RedisConnectionString"), true);
                multConfig.Password = configuration.GetValue<string>("Redis:password");
                return ConnectionMultiplexer.Connect(multConfig);
            });
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
