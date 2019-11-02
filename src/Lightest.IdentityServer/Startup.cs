using System.Collections.Generic;
using System.Linq;
using IdentityServer4;
using IdentityServer4.Models;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.IdentityServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: ApiController]

namespace Lightest.IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public static IEnumerable<ApiResource> GetApiResources() => new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "api",
                    DisplayName = "Api",
                    Scopes =
                    {
                        new Scope("api", "API")
                    }
                }
            };

        public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("Login");

            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(e => e.MapControllers());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddDbContext<RelationalDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("Relational"), b => b.MigrationsAssembly("Lightest.Api")));
            services.AddCors(options =>
            {
                options.AddPolicy("General", b =>
                {
                    b.AllowAnyHeader();
                    b.AllowAnyMethod();
                    b.AllowAnyOrigin();
                });
                options.AddPolicy("Login", b =>
                {
                    b.AllowAnyMethod();
                    b.AllowAnyHeader();
                    b.AllowCredentials();
                    var origins = Configuration.GetSection("URIs").GetChildren()
                        .Select(c => c.Value).ToArray();
                    b.WithOrigins(origins);
                });
            });
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<RelationalDbContext>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(o =>
                o.Cookie.Domain = Configuration.GetSection("Domain").Value);
            services.AddIdentityServer(Configuration.GetSection("IdentityServer"))
                .AddDeveloperSigningCredential()
                .AddDefaultEndpoints()
                .AddAspNetIdentity<ApplicationUser>()
                .AddInMemoryClients(Clients)
                .AddInMemoryIdentityResources(GetIdentityResources())
                .AddInMemoryApiResources(GetApiResources())
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(Configuration.GetConnectionString("Relational"),
                        s => s.MigrationsAssembly("Lightest.IdentityServer"));
                    options.EnableTokenCleanup = true;
                })
                .AddProfileService<ProfileService>();
        }

        public IEnumerable<Client> Clients
        {
            get
            {
                var uris = Configuration.GetSection("URIs").GetChildren();
                var links = new List<string>();
                foreach (var uri in uris)
                {
                    links.Add(uri.Value);
                }
                return new List<Client>
                {
                    new Client
                    {
                        ClientId = "client",
                        ClientName = "client",
                        AllowedScopes = {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "api"
                        },
                        AllowedGrantTypes = GrantTypes.Implicit,
                        AllowOfflineAccess = true,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris = links,
                        RequireConsent = false,
                        AccessTokenType = AccessTokenType.Jwt
                    }
                };
            }
        }
    }
}
