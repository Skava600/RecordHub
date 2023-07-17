using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.BasketService.Application.Mappers;
using RecordHub.BasketService.Application.Protos;
using RecordHub.BasketService.Application.Services;
using RecordHub.BasketService.Application.Validators;
using RecordHub.BasketService.Infrastructure.Config;
using RecordHub.BasketService.Infrastructure.Data.Repositories;
using RecordHub.BasketService.Infrastructure.Services;
using RecordHub.Shared.Extensions;
using StackExchange.Redis;

namespace RecordHub.BasketService.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRedisPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var cfg = configuration.Get<AppConfig>();
            services.Configure<RedisConfig>(
                    configuration.GetSection(
                        key: nameof(RedisConfig)));

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

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var cfg = configuration.Get<AppConfig>();

            services.AddGrpcServices(cfg.GrpcConfig);

            services.AddScoped<IBasketService, RecordHub.BasketService.Infrastructure.Services.BasketService>();
            services.AddScoped<ICatalogGrpcClient, CatalogGrpcClient>();

            services.AddValidatorsFromAssemblyContaining(typeof(BasketItemValidator));

            services.AddMassTransit(cfg.MassTransit);

            services.AddAutoMapper(typeof(CheckoutProfile));

            return services;
        }

        private static IServiceCollection AddGrpcServices(this IServiceCollection services, GrpcConfig config)
        {
            services
                .AddGrpcClient<CatalogChecker.CatalogCheckerClient>(o =>
                {
                    o.Address = new Uri(config.CatalogUrl);
                    o.ChannelOptionsActions.Clear();
                    o.ChannelOptionsActions.Add((opt) =>
                    {
                        opt.UnsafeUseInsecureChannelCallCredentials = true;
                    });
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    return handler;
                });

            return services;
        }

        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfiguredJwtBearer(configuration);
            services.AddAuthorization();

            return services;
        }

        private static IServiceCollection AddMassTransit(this IServiceCollection services, MassTransitOptions transitOptions)
        {
            services.AddMassTransit(x =>
            {
                x.AddBus(context => Bus.Factory.CreateUsingRabbitMq((cfg) =>
                {
                    cfg.Host(transitOptions.Host, h =>
                    {
                        h.Username(transitOptions.Username);
                        h.Password(transitOptions.Password);
                    });
                }));

            });

            return services;
        }
    }
}
