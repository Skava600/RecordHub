﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RecordHub.IdentityService.Tests.IntegrationTests.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static void RemoveDbContext<T>(this IServiceCollection services) where T : DbContext
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
            if (descriptor != null) services.Remove(descriptor);
        }

        public static void EnsureDbCreated<T>(this IServiceCollection services) where T : DbContext
        {
            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<T>();
            var pendingMigrations = context.Database.GetPendingMigrations();

            if (pendingMigrations.Any())
            {
                context.Database.Migrate();
            }
        }
    }
}
