using IdentityServer4.AccessTokenValidation;
using Lightest.Api.Services;
using Lightest.Api.Services.AccessServices;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lightest.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lightest API V1");
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<RelationalDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("Relational"), b => b.MigrationsAssembly("Lightest.Api"));
            });

            var auth = Configuration.GetSection("Authority").Value;

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.ApiName = "api";
                    options.Authority = auth;
                    options.RequireHttpsMetadata = false;
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Lightest API", Version = "1" });
            });
            services.AddSingleton<IServerRepository, ServerRepository>();
            services.AddSingleton<ITransferServiceFactory, TransferServiceFactory>();
            services.AddTransient<ITestingService, TestingService>();
            services.AddTransient<IAccessService<Category>, CategoriesAccessService>();
            services.AddTransient<IAccessService<Group>, GroupsAccessService>();
            services.AddTransient<IAccessService<TaskDefinition>, TasksAccessService>();
            services.AddTransient<IAccessService<Language>, LanguagesAccessService>();
            services.AddTransient<IAccessService<IUpload>, UploadsAccessService>();
        }
    }
}
