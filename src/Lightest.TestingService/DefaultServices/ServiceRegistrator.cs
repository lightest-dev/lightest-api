using Lightest.TestingService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.TestingService.DefaultServices
{
    public static class ServiceRegistrator
    {
        public static IServiceCollection AddDefaultTestingServices(this IServiceCollection services)
        {
            services.AddSingleton<ITransferServiceFactory, TransferServiceFactory>();
            services.AddSingleton<IUploadProcessorFactory, UploadProcessorFactory>();
            services.AddSingleton<ITestingRunner, TestingRunner>();
            services.AddScoped<ITestingService, DefaultTestingService>();
            return services;
        }
    }
}
