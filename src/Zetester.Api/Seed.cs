using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.EntityFramework.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zetester.Data;
using Zetester.Data.Models;

namespace Zetester.Api
{
    public static class Seed
    {
        public static void EnsureDataSeeded(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                SeedConfig(scope);
                SeedRelational(scope);
            }
        }

        private static void SeedConfig(IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            context.Database.Migrate();
            if (!context.Clients.Any())
            {
                /*foreach (var client in Config.GetClients().ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }*/
                context.SaveChanges();
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
