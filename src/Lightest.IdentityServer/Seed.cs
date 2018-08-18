using System;
using Lightest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.IdentityServer
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
            //todo
            return;
            var context = scope.ServiceProvider.GetRequiredService<RelationalDbContext>();
            context.Database.Migrate();
            context.SaveChanges();
        }
    }
}
