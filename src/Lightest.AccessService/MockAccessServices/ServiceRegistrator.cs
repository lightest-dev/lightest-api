using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.AccessService.MockAccessServices
{
    public static class MockServiceRegistrator
    {
        public static IServiceCollection AddMockAccess(this IServiceCollection services)
        {
            services.AddTransient<IAccessService<Category>, CategoriesAccessService>();
            services.AddTransient<IAccessService<Group>, GroupsAccessService>();
            services.AddTransient<IAccessService<TaskDefinition>, TasksAccessService>();
            services.AddTransient<IAccessService<Language>, LanguagesAccessService>();
            services.AddTransient<IAccessService<IUpload>, UploadsAccessService>();
            services.AddTransient<IAccessService<ApplicationUser>, ProfileAccessService>();
            services.AddTransient<IAccessService<Checker>, CheckerAccessService>();
            return services;
        }
    }
}
