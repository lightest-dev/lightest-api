using Lightest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

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
            /*var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
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
            }*/
            context.SaveChanges();
        }
    }
}