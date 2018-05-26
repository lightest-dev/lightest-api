using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Lightest.Data;
using Lightest.Data.Models;

namespace Lightest.Api
{
    public static class Seed
    {
        public static void EnsureDataSeeded(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                SeedRelational(scope);
            }
        }

        private static void SeedRelational(IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetRequiredService<RelationalDbContext>();
            context.Database.Migrate();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var testUser = userManager.FindByNameAsync("test").Result;
            if (testUser == null)
            {
                testUser = new ApplicationUser
                {
                    UserName = "test"
                };
                var result = userManager.CreateAsync(testUser, "Password12$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
            context.SaveChanges();
        }
    }
}