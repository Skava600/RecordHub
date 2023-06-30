using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.OrderingService.Application.Services;
using RecordHub.OrderingService.Infrastructure.Config;
using RecordHub.Shared.Extensions;
using RecordHub.Shared.MassTransit.Models.Order;

namespace RecordHub.OrderingService.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            var cfg = configuration.Get<AppConfig>();
            services.AddScoped<IOrderingService, RecordHub.OrderingService.Infrastructure.Services.OrderingService>();
            return services.AddMassTransit(cfg.MassTransit);
        }
        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfiguredJwtBearer(configuration);
            services.AddAuthorization();

            return services;
        }

        private static IServiceCollection AddMassTransit(this IServiceCollection services, MassTransitOptions cfg)
        {
            services.AddMassTransit(x =>
            {



                x.AddBus(context => Bus.Factory.CreateUsingRabbitMq((c) =>
                {
                    c.Host(cfg.Host, h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    c.ConfigureEndpoints(context);
                }));
                x.AddRequestClient<BasketInfoRequest>();
            });

            return services;
        }
    }
}
