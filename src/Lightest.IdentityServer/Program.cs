using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Lightest.IdentityServer
{
    public class Program
    {
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
.UseStartup<Startup>()
.ConfigureAppConfiguration((context, config) =>
{
    config.AddJsonFile("dbsettings.json");
});
        }

        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            if (args.Contains("--seed"))
            {
                Seed.EnsureDataSeeded(host.Services);
            }
            host.Run();
        }
    }
}
