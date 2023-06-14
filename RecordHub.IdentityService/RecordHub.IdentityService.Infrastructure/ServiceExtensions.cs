using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Core.Services.Logging;
using RecordHub.IdentityService.Infrastructure.Configuration;
using RecordHub.IdentityService.Infrastructure.Services;
using RecordHub.IdentityService.Infrastructure.Services.Logging;
using System.Text;

namespace RecordHub.IdentityService.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtConfig>(configuration.GetSection("Jwt"));
            services.AddSingleton<JwtConfig>(sp => sp.GetRequiredService<IOptions<JwtConfig>>().Value);

            services.AddAuthentication(conf =>
            {
                conf.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                conf.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("Bearer", options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration.GetValue<string>("Jwt:Issuer"),
                    ValidateAudience = true,
                    ValidAudience = configuration.GetValue<string>("Jwt:Audience"),
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Key") ?? "mysecretkey123")),
                    ValidateIssuerSigningKey = true,
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers["Authorization"].ToString();
                        if (!string.IsNullOrEmpty(token) && !token.StartsWith("Bearer "))
                        {
                            context.Request.Headers["Authorization"] = "Bearer " + token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
            services.RegisterLoggingInterfaces();

            return services;

        }

        private static IServiceCollection RegisterLoggingInterfaces(this IServiceCollection services)
        {
            services.AddScoped(typeof(IAppLogging<>), typeof(AppLogging<>));
            return services;
        }
    }
}
