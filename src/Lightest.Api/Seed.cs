using System;
using Lightest.Data;
using Lightest.Data.Seeding.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.Api
{
    public static class Seed
    {
        public static void EnsureDataSeeded(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                SeedRelational(scope);
            }
        }

        private static void SeedRelational(IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetRequiredService<RelationalDbContext>();
            context.Database.Migrate();
            var seeder = scope.ServiceProvider.GetRequiredService<ISeeder>();
            seeder.Seed();
            seeder.AddTestData();
            context.SaveChanges();
        }
    }
}
