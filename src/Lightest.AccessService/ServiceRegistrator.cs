using Lightest.AccessService.MockAccessServices;
using Lightest.AccessService.RoleBasedAccessServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.AccessService
{
    public static class ServiceRegistrator
    {
        public static IServiceCollection AddAccessServices(this IServiceCollection services, IConfigurationSection configuration)
        {
            var mode = configuration?.Value?.ToLower();
            if (mode == "mock")
            {
                services.AddMockAccess();
            }
            else
            {
                services.AddRoleBasedAccess();
            }
            return services;
        }
    }
}
