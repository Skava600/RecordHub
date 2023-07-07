﻿using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RecordHub.BasketService.Applicatation.Mappers;
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

            services.AddGrpcServices(cfg.GrpcConfig);
            services.AddScoped<IBasketService, RecordHub.BasketService.Infrastructure.Services.BasketService>();
            services.AddValidatorsFromAssemblyContaining(typeof(ShoppingCartItemValidator));
            services.AddMassTransit(configuration, cfg.MassTransit);
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

        private static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration, MassTransitOptions transitOptions)
        {
            services.AddMassTransit(x =>
            {
                x.AddBus(context => Bus.Factory.CreateUsingRabbitMq((cfg) =>
                {
                    cfg.Host(transitOptions.Host, h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });


                }));

            });

            return services;
        }
    }
}
