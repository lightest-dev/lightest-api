using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Lightest.IdentityServer
{
    public class Program
    {
        public static IHostBuilder CreateWebHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>()

                    .ConfigureKestrel((options) =>
                    {
                        //listen to localhost only, reverse proxy is used for outside comunication
                        options.ListenLocalhost(5200, listenOptions =>
                        {
                            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                            //not working on linux
                            //listenOptions.UseHttps();
                        });
                    });
                })
            .UseSystemd()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("settings.private.json");
            });

        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }
    }
}
