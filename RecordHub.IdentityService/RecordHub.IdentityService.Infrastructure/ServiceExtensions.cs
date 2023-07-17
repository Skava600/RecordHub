using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecordHub.IdentityService.Core.Mappers;
using RecordHub.IdentityService.Core.Publishers;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Core.Validators;
using RecordHub.IdentityService.Infrastructure.Configuration;
using RecordHub.IdentityService.Infrastructure.Publishers;
using RecordHub.IdentityService.Infrastructure.Services;
using RecordHub.Shared.MassTransit.Models;
using RecordHub.Shared.Services.Logging;
using System.Text;

namespace RecordHub.IdentityService.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var cfg = configuration.Get<AppConfig>();
            services.AddJwtBearerAuth(configuration);
            services.RegisterLoggingInterfaces();
            services.InjectInfrastructureServices(configuration);
            services.AddMassTransit(cfg.MassTransit);

            return services;
        }

        private static IServiceCollection AddJwtBearerAuth(this IServiceCollection services, IConfiguration configuration)
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Key"))),
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

            return services;
        }

        private static IServiceCollection InjectInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddAutoMapper(typeof(UserProfile));
            services.AddAutoMapper(typeof(AddressProfile));

            services.AddValidatorsFromAssemblyContaining(typeof(AddressValidator));

            return services;
        }

        private static IServiceCollection AddMassTransit(this IServiceCollection services, MassTransitOptions transitOptions)
        {
            services.AddScoped<IPublisher<MailData>, SendEmailPublisher>();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(transitOptions.Host, h =>
                    {
                        h.Username(transitOptions.Username);
                        h.Password(transitOptions.Password);
                    });
                });
            });

            return services;
        }

        private static IServiceCollection RegisterLoggingInterfaces(this IServiceCollection services)
        {
            services.AddScoped(typeof(IAppLogging<>), typeof(AppLogging<>));

            return services;
        }
    }
}
