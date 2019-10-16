using Lightest.TestingService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.TestingService.DefaultServices
{
    public static class ServiceRegistrator
    {
        public static IServiceCollection AddDefaultTestingServices(this IServiceCollection services)
        {
            services.AddScoped<IServerRepository, ServerRepository>();
            services.AddScoped<ITransferServiceFactory, TransferServiceFactory>();
            services.AddScoped<ITestingService, TestingService>();
            return services;
        }
    }
}
