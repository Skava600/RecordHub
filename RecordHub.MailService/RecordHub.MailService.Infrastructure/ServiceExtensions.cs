﻿using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.MailService.Application.Services;
namespace RecordHub.MailService.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddTransient<IMailService, RecordHub.MailService.Infrastructure.Services.MailService>();
            return services;
        }

        public static IServiceCollection AddMassTransit(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("messagebus", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });
            return services;
        }
    }
}
