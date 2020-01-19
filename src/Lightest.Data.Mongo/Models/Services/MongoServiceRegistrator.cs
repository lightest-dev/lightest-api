using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.Data.Mongo.Models.Services
{
    public static class MongoServiceRegistrator
    {
        public static IServiceCollection AddMongoCodeRepository(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IUploadDataRepository, UploadDataService>();
            services.Configure<UploadDataService>(config);
            return services;
        }
    }
}
