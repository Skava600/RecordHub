using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.ChatService.Application.Services;
using RecordHub.ChatService.Infrastructure.Config;
using RecordHub.ChatService.Infrastructure.Data.Repos;
using RecordHub.ChatService.Infrastructure.Services;

namespace RecordHub.ChatService.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMongoDbPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ChatStoreDatabaseConfig>(configuration.GetSection(
                        key: nameof(ChatStoreDatabaseConfig)));
            services.AddSingleton<IRoomsRepository, RoomsRepository>();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<IRoomsService, RoomsService>();

            return services;
        }
    }
}
