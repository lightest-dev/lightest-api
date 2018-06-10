using IdentityServer4;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Models;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Lightest.Api
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
            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters()
                .AddApiExplorer()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                
            services.AddDbContext<RelationalDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("Relational"), b => b.MigrationsAssembly("Lightest.Api"));
            });
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<RelationalDbContext>()
                .AddDefaultTokenProviders();
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddDefaultEndpoints()
                .AddAspNetIdentity<ApplicationUser>()
                .AddInMemoryClients(GetClients())
                .AddInMemoryIdentityResources(GetIdentityResources())
                .AddInMemoryApiResources(GetApiResources())
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(Configuration.GetConnectionString("Relational"), 
                        s => s.MigrationsAssembly("Lightest.Api"));
                    options.EnableTokenCleanup = true;
                });
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.ApiName = "api";
                    //todo: add api url
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Lightest API", Version = "1" });
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
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lightest API V1");
            });
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource()
                {
                    Name = "api",
                    DisplayName = "Api",
                    Scopes =
                    {
                        new Scope("api", "API")
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client()
                {
                    ClientId = "client",
                    ClientName = "client",
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "api"
                    },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = { "https://lightest.tk", "https://localhost" },
                    RequireConsent = false,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AccessTokenType = AccessTokenType.Jwt
                }
            };
        }
    }
}