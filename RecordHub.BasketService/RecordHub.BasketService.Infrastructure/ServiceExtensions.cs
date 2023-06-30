using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RecordHub.BasketService.Applicatation.Services;
using RecordHub.BasketService.Applicatation.Validators;
using RecordHub.BasketService.Infrastructure.Config;
using RecordHub.BasketService.Infrastructure.Consumers;
using RecordHub.BasketService.Infrastructure.Data.Repositories;
using RecordHub.Shared.Extensions;
using StackExchange.Redis;

namespace RecordHub.BasketService.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRedisPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var cfg = configuration.Get<AppConfig>();
            services.AddSingleton<RedisConfig>(sp => sp.GetRequiredService<IOptions<RedisConfig>>().Value);
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var multConfig = ConfigurationOptions.Parse($"{cfg.RedisConfig.Host}:{cfg.RedisConfig.Port}", true);
                multConfig.AbortOnConnectFail = false;
                multConfig.Password = configuration.GetValue<string>("RedisConfig:password");
                return ConnectionMultiplexer.Connect(multConfig);
            });
            services.AddScoped<IBasketRepository, BasketRepository>();
            return services;
        }

        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            var cfg = configuration.Get<AppConfig>();

            services.AddScoped<IBasketService, RecordHub.BasketService.Infrastructure.Services.BasketService>();
            services.AddValidatorsFromAssemblyContaining(typeof(ShoppingCartItemValidator));
            services.AddMassTransit(configuration, cfg.MassTransit);
            return services;
        }

        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfiguredJwtBearer(configuration);
            services.AddAuthorization();

            return services;
        }

        private static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration, MassTransitOptions transitOptions)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<BasketInfoRequestConsumer>()
                 .Endpoint(e => e.Name = "basket-info");

                x.AddBus(context => Bus.Factory.CreateUsingRabbitMq((cfg) =>
                {
                    cfg.Host(transitOptions.Host, h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint(transitOptions.Queue, e =>
                    {
                        e.PrefetchCount = 16;
                        e.UseMessageRetry(r => r.Interval(2, 3000));
                        e.ConfigureConsumer<BasketInfoRequestConsumer>(context);
                    });

                }));

            });

            return services;
        }
    }
}
