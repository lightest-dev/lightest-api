using Lightest.TestingService.DefaultServices;
using Lightest.TestingService.Factories;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.TestingService
{
    public static class ServiceRegistrator
    {
        public static IServiceCollection AddDefaultTestingServices(this IServiceCollection services)
        {
            services.AddSingleton<ITransferServiceFactory, TransferServiceFactory>();
            services.AddSingleton<IUploadProcessorFactory, UploadProcessorFactory>();
            services.AddSingleton<ITestingRunner, TestingRunner>();
            services.AddScoped<ITestingService, DefaultTestingService>();
            services.AddHostedService<TestingWorker>();
            return services;
        }
    }
}
