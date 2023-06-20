using RecordHub.MailService.Infrastructure.Settings;
namespace RecordHub.MailService.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureMail(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
            return services;
        }
    }
}
