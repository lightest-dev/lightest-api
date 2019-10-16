using System;
using System.Threading.Tasks;
using Lightest.Data;
using Lightest.Data.Seeding.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.Api
{
    public static class Seed
    {
        public static async Task EnsureDataSeeded(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            await SeedRelational(scope);
        }

        private static async Task SeedRelational(IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetRequiredService<RelationalDbContext>();
            context.Database.Migrate();
            var seeder = scope.ServiceProvider.GetRequiredService<ISeeder>();
            await seeder.Seed();
            await seeder.AddTestData();
            context.SaveChanges();
        }
    }
}
