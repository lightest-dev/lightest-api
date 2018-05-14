using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zetester.Data;
using Zetester.Data.Models;

namespace Zetester.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDbContext<RelationalDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("Relational"), b => b.MigrationsAssembly("Zetester.Api"));
            });
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<RelationalDbContext>()
                .AddDefaultTokenProviders();
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddDefaultEndpoints()
                .AddAspNetIdentity<ApplicationUser>()
                .AddInMemoryClients(Configuration.GetSection("clients"))
                .AddInMemoryIdentityResources(Configuration.GetSection("identity_resources"))
                .AddInMemoryApiResources(Configuration.GetSection("api_resources"))
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(Configuration.GetConnectionString("Relational"), s => s.MigrationsAssembly("Zetester.Api"));
                    options.EnableTokenCleanup = true;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseIdentityServer();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}