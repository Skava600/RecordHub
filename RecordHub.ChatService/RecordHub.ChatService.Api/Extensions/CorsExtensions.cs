namespace RecordHub.ChatService.Api.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddClientCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyMethod().AllowAnyHeader()
                .WithOrigins(configuration.GetValue<string>("ClientHost"))
                .AllowCredentials();
            }));

            return services;
        }
    }
}
