using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.Data.Mongo.Models.Services
{
    public class MongoServiceRegistrator
    {
        public static IServiceCollection AddMongoCodeRepository(IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IUploadDataRepository, UploadDataService>();
            return services;
        }
    }
}
