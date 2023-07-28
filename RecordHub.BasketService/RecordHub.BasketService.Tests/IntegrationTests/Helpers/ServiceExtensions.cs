using MassTransit;
using Microsoft.Extensions.DependencyInjection;
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

        public static IServiceCollection AddMockedMassTransit(this IServiceCollection services)
        {
            var publishEndpoint = new Mock<IPublishEndpoint>();
            services.AddScoped<IPublishEndpoint>(e => publishEndpoint.Object);

            return services;
        }

        public static IServiceCollection RemoveApplicationRedis(this IServiceCollection services)
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
