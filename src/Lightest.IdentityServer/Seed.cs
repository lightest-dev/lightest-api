using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.IdentityServer
{
    public static class Seed
    {
        public static void EnsureDataSeeded(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                SeedRelational(scope).Wait();
            }
        }

        private static async Task SeedRelational(IServiceScope scope)
        {
            try
            {
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                if (await roleManager.FindByNameAsync("Admin") == null)
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                if (await roleManager.FindByNameAsync("Teacher") == null)
                {
                    await roleManager.CreateAsync(new IdentityRole("Teacher"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                if (!userManager.Users.Any())
                {
                    var user = new ApplicationUser
                    {
                        UserName = "test",
                        Email = "test@mail.com"
                    };
                    await userManager.CreateAsync(user, "Password12$");
                }

                var dbUser = await userManager.FindByNameAsync("test");

                if (!await userManager.IsInRoleAsync(dbUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(dbUser, "Admin");
                }

                if (!await userManager.IsInRoleAsync(dbUser, "Teacher"))
                {
                    await userManager.AddToRoleAsync(dbUser, "Teacher");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
