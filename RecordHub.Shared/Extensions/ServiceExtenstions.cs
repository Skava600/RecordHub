using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecordHub.Shared.Config;
using System.Text;

namespace RecordHub.Shared.Extensions
{
    public static class ServiceExtenstions
    {
        public static IServiceCollection AddConfiguredJwtBearer(this IServiceCollection services, IConfiguration configuration)
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
    }
}
