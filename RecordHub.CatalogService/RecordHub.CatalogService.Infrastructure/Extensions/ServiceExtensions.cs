using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.Mappers;
using RecordHub.CatalogService.Application.Services;
using RecordHub.CatalogService.Application.Validators;
using RecordHub.CatalogService.Infrastructure.Config;
using RecordHub.CatalogService.Infrastructure.Services;
using RecordHub.Shared.Extensions;
using System.Reflection;

namespace RecordHub.CatalogService.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapperProfiles();
            services.AddRedisCaching(configuration);

            services.AddValidatorsFromAssemblyContaining(typeof(RecordValidator));

            services.AddScoped<IRecordCatalogService, RecordCatalogService>();
            services.AddScoped<IArtistCatalogService, ArtistCatalogService>();
            services.AddScoped<IStyleCatalogService, StyleCatalogService>();
            services.AddScoped<ILabelCatalogService, LabelCatalogService>();
            services.AddScoped<ICountryCatalogService, CountryCatalogService>();

            return services;
        }

        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfiguredJwtBearer(configuration);
            services.AddAuthorization();

            return services;
        }

        private static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
        {
            services.AddScoped(provider =>
               new MapperConfiguration(cfg =>
               {
                   cfg.AddProfile(new RecordProfile(
                      provider.GetService<IUnitOfWork>()));
                   cfg.AddProfile(new ArtistProfile());
                   cfg.AddProfile(new StyleProfile());
                   cfg.AddProfile(new CountryProfile());
                   cfg.AddProfile(new LabelProfile());
               })
               .CreateMapper());

            return services;
        }

        private static IServiceCollection AddRedisCaching(this IServiceCollection services, IConfiguration configuration)
        {
            var cfg = configuration.Get<AppConfig>();
            services.Configure<RedisConfig>(
                configuration.GetSection(
                    key: nameof(RedisConfig)));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cfg.RedisConfig.Host;
                options.InstanceName = Assembly.GetAssembly(typeof(ServiceExtensions)).FullName;
            });

            services.AddScoped<IRedisCacheService, RedisCacheService>();

            return services;
        }
    }
}