using System;
using Lightest.CodeManagment.Entity;
using Lightest.CodeManagment.Models;
using Lightest.Data.CodeManagment.InMemory;
using Lightest.CodeManagment.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.Data.CodeManagment.Services
{
    public static class CodeManagmentServiceRegistrator
    {
        public static readonly string CodeManagmentConfigSection = "CodeManagment";

        public static IServiceCollection AddMongoCodeManagmentService(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<ICodeManagmentService, MongoCodeManagmentService>();
            services.Configure<MongoCodeManagmentService>(config);
            return services;
        }

        public static IServiceCollection AddInMemoryCodeManagmentService(this IServiceCollection services)
        {
            services.AddSingleton<ICodeManagmentService, InMemoryCodeManagmentService>();
            return services;
        }

        public static IServiceCollection AddEntityCodeManagmentService(this IServiceCollection services)
        {
            services.AddScoped<ICodeManagmentService, EntityCodeManagmentService>();
            return services;
        }

        public static IServiceCollection AddCodeManagmentService(this IServiceCollection services, IConfiguration config)
        {
            var settings = config.GetSection(CodeManagmentConfigSection).Get<CodeManagmentSettings>();
            services.Configure<CodeManagmentSettings>(config.GetSection(CodeManagmentConfigSection));
            switch(settings.Mode)
            {
                case CodeManagmentMode.Entity:
                    {
                        return AddEntityCodeManagmentService(services);
                    }
                case CodeManagmentMode.Mongo:
                    {
                        return AddMongoCodeManagmentService(services, config);
                    }
                case CodeManagmentMode.InMemory:
                    {
                        return AddInMemoryCodeManagmentService(services);
                    }
                default:
                    {
                        throw new ArgumentException(nameof(config), "Code Managment Mode is not supported");
                    }
            }
        }
    }
}
