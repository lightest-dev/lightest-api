using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Lightest.Api
{
    public static class Program
    {
        public static IHostBuilder CreateWebHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>()
                    .ConfigureAppConfiguration((context, config) => config.AddJsonFile("settings.private.json"))
                    .ConfigureKestrel((options) =>
                    {
                        //listen to localhost only, reverse proxy is used for outside comunication
                        options.ListenLocalhost(5100, listenOptions =>
                        {
                            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                            //not working on linux
                            //listenOptions.UseHttps();
                        });
                    });
                });

        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            if (args.Contains("--seed"))
            {
                host.Services.EnsureDataSeeded().GetAwaiter().GetResult();
            }
            host.Run();
        }
    }
}
