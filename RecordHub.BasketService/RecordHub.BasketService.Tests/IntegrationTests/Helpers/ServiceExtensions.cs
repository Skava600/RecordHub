using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecordHub.BasketService.Application.Protos;
using RecordHub.BasketService.Application.Services;
using StackExchange.Redis;

namespace RecordHub.BasketService.Tests.IntegrationTests.Helpers
{
    internal static class ServiceExtensions
    {
        public static IServiceCollection AddMockedGrpc(this IServiceCollection services)
        {
            var grpcClientMocked = new Mock<ICatalogGrpcClient>();
            grpcClientMocked.Setup(client => client.CheckProductExistenceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ProductReply { IsExisting = true, Name = "Some record", Price = 100 });
            services.AddScoped<ICatalogGrpcClient>(a => grpcClientMocked.Object);

            return services;
        }

        public static IServiceCollection AddMockedRabbitMq(this IServiceCollection services)
        {
            var grpcClientMocked = new Mock<ICatalogGrpcClient>();
            grpcClientMocked.Setup(client => client.CheckProductExistenceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ProductReply { IsExisting = true, Name = "Some record", Price = 100 });
            services.AddScoped<ICatalogGrpcClient>(a => grpcClientMocked.Object);

            return services;
        }

        public static IServiceCollection RemoveApplicationMassTransit(this IServiceCollection services)
        {
            var massTransitHostedService = services.FirstOrDefault(d => d.ServiceType == typeof(IHostedService) &&
                   d.ImplementationFactory != null &&
                   d.ImplementationFactory.Method.ReturnType == typeof(MassTransitHostedService)
               );
            services.Remove(massTransitHostedService);
            var descriptors = services.Where(d =>
                   d.ServiceType.Namespace.Contains("MassTransit", StringComparison.OrdinalIgnoreCase))
                                      .ToList();
            foreach (var d in descriptors)
            {
                services.Remove(d);
            }

            return services;
        }

        public static IServiceCollection RemoveProductionRedis(this IServiceCollection services)
        {
            var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(IConnectionMultiplexer));

            services.Remove(dbConnectionDescriptor);

            return services;
        }

        public static IServiceCollection AddTestContainersRedis(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var multConfig = ConfigurationOptions.Parse(connectionString, true);
                multConfig.AbortOnConnectFail = false;

                return ConnectionMultiplexer.Connect(multConfig);
            });

            return services;
        }
    }
}
