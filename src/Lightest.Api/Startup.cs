using IdentityServer4.AccessTokenValidation;
using Lightest.AccessService;
using Lightest.Api.Extensions;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Seeding;
using Lightest.Data.Seeding.Interfaces;
using Lightest.TestingService.DefaultServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: ApiController]

namespace Lightest.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseCors("General");
            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthorization();
            app.UseEndpoints(e =>
            {
                e.MapControllers().RequireAuthorization();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lightest API V1");
                c.RoutePrefix = string.Empty;
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(
                                     new SlugifyParameterTransformer()));
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddDbContext<RelationalDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("Relational"), b => b.MigrationsAssembly("Lightest.Api"));
            });

            var auth = Configuration.GetSection("Authority").Value;

            services.AddIdentityWithoutAuthenticator<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<RelationalDbContext>();

            services
                .AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                })
                .AddIdentityServerAuthentication(options =>
                {
                    options.ApiName = "api";
                    options.Authority = auth;
                    options.RequireHttpsMetadata = false;
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Lightest API", Version = "1" });
            });
            services.AddHttpContextAccessor();

            services.AddCors(options =>
            {
                options.AddPolicy("General", b =>
                {
                    b.AllowAnyHeader();
                    b.AllowAnyMethod();
                    b.AllowAnyOrigin();
                });
            });

            services.AddDefaultTestingServices();
            services.AddAccessServices(Configuration.GetSection("AccessMode"));
            services.AddTransient<ISeeder, DefaultSeeder>();
        }
    }
}
