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
                })
                .ConfigureKestrel((options) =>
                {
                    //listen to localhost only, reverse proxy is used for outside comunication
                    options.ListenLocalhost(5200, listenOptions =>
                    {
                        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                        listenOptions.UseHttps("localcert.pfx");
                    });
                });
        }

        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            if (args.Contains("--seed"))
            {
                host.Services.EnsureDataSeeded();
            }
            host.Run();
        }
    }
}
