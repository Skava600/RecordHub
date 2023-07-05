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
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddJwtBearerAuth(configuration);
            services.RegisterLoggingInterfaces();
            services.InjectCoreServices();

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

            return services;
        }

        private static IServiceCollection InjectCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IPublisher<MailData>, SendEmailPublisher>();

            services.AddAutoMapper(typeof(UserProfile));
            services.AddAutoMapper(typeof(AddressProfile));

            services.AddValidatorsFromAssemblyContaining(typeof(AddressValidator));

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq://messagebus", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
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
