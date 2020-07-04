using System;
using Lightest.TestingService.DefaultServices;
using Lightest.TestingService.Factories;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.Models;
using Lightest.TestingService.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.TestingService
{
    public static class ServiceRegistrator
    {
        private const string GrpcSettingsPath = "Grpc";

        public static IServiceCollection AddDefaultTestingServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<GrpcSettings>(config.GetSection(GrpcSettingsPath));
            var settings = config.GetSection(GrpcSettingsPath).Get<GrpcSettings>();
            if (settings.Insecure)
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }

            services.AddSingleton<ITransferServiceFactory, TransferServiceFactory>();
            services.AddSingleton<IUploadProcessorFactory, UploadProcessorFactory>();
            services.AddSingleton<ITestingRunner, TestingRunner>();
            services.AddScoped<ITestingService, DefaultTestingService>();
            services.AddHostedService<TestingWorker>();
            return services;
        }
    }
}
