using Lightest.TestingService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.TestingService.DefaultServices
{
    public static class ServiceRegistrator
    {
        public static IServiceCollection AddDefaultTestingServices(this IServiceCollection services)
        {
            services.AddSingleton<IServerRepository, ServerRepository>();
            services.AddSingleton<ITransferServiceFactory, TransferServiceFactory>();
            services.AddSingleton<ITestingService, TestingService>();
            return services;
        }
    }
}
