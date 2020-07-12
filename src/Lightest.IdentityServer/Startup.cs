using System.Collections.Generic;
using System.Linq;
using IdentityServer4;
using IdentityServer4.Models;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.IdentityServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
        {
            new ApiScope
            {
                Name = "api",
                DisplayName = "Api"
            }
        };

        public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "api",
                    DisplayName = "Api",
                    Scopes =
                    {
                        "api"
                    }
                }
            };

        public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>
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

            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };

            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(forwardOptions);
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

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 5;
            });

            services.ConfigureApplicationCookie(o =>
                o.Cookie.Domain = Configuration.GetSection("CookieDomain").Value);
            services.AddIdentityServer(Configuration.GetSection("IdentityServer"))
                .AddDeveloperSigningCredential()
                .AddDefaultEndpoints()
                .AddAspNetIdentity<ApplicationUser>()
                .AddInMemoryClients(Clients)
                .AddInMemoryApiScopes(ApiScopes)
                .AddInMemoryIdentityResources(IdentityResources)
                .AddInMemoryApiResources(ApiResources)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(Configuration.GetConnectionString("Relational"),
                        s => s.MigrationsAssembly("Lightest.IdentityServer"));
                    options.EnableTokenCleanup = true;
                })
                .AddProfileService<ProfileService>();
            services.AddTransient<IPasswordGenerator, PasswordGenerator>();
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
