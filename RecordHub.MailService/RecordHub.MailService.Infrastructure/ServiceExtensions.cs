namespace RecordHub.MailService.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IMailService, RecordHub.MailService.Infrastructure.Services.MailService>();
            services.AddHangfire(
                x => x.UsePostgreSqlStorage(
                    configuration.GetConnectionString("HangfireConnection")));
            services.AddHangfireServer();
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseSerilogLogProvider()
                .UseColouredConsoleLogProvider()
                .UseRecommendedSerializerSettings();

            return services;
        }

        public static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<SendEmailConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration.GetValue<string>("MassTransit:Host"), h =>
                    {
                        h.Username(configuration.GetValue<string>("MassTransit:Username"));
                        h.Password(configuration.GetValue<string>("MassTransit:Password"));
                    });


                    cfg.ReceiveEndpoint(configuration.GetValue<string>("MassTransit:EmailQueue"), ep =>
                    {
                        ep.ConfigureConsumer<SendEmailConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
