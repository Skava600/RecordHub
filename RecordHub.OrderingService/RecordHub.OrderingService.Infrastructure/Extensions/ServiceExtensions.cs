using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.OrderingService.Application.Mappers;
using RecordHub.OrderingService.Application.Services;
using RecordHub.OrderingService.Infrastructure.Config;
using RecordHub.OrderingService.Infrastructure.Consumers;
using RecordHub.Shared.Extensions;

namespace RecordHub.OrderingService.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            var cfg = configuration.Get<AppConfig>();
            services.AddScoped<IOrderingService, RecordHub.OrderingService.Infrastructure.Services.OrderingService>();
            services.AddAutoMapper(typeof(OrderProfile));
            return services.AddMassTransit(cfg.MassTransit);
        }
        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfiguredJwtBearer(configuration);
            services.AddAuthorization();

            return services;
        }

        private static IServiceCollection AddMassTransit(this IServiceCollection services, MassTransitOptions massTransitOptions)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<BasketCheckoutConsumer>();
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(massTransitOptions.Host, h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint(massTransitOptions.BasketCheckoutQueue, c =>
                    {
                        c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
                    });
                });
            });

            return services;
        }
    }
}
