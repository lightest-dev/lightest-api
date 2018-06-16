using IdentityServer4.EntityFramework.DbContexts;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

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