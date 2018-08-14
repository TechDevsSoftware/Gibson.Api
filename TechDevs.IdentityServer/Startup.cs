using System;
using IdentityServer4;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TechDevs.Accounts;
using MongoDB.Bson.Serialization;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using IdentityServer4.Validation;

namespace TechDevs.IdentityServer
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
                options.AuthenticationDisplayName = "Windows";
            });

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            });

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // in-memory, code config
            builder.AddInMemoryIdentityResources(Config.GetIdentityResources());
            builder.AddInMemoryApiResources(Config.GetApis());
            builder.AddInMemoryClients(Config.GetClients());

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                // For now we use the developer signing credential
                builder.AddDeveloperSigningCredential();

                //throw new Exception("need to configure key material");
            }

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = "562400802513-l81m74td45m43m3r8mj9qq3ipdg9o071.apps.googleusercontent.com";
                    options.ClientSecret = "uhYVb-Llb2RJlXnGWuX_nC_A";
                });



            // TechDevs Account DLL Wire up ---  Eventually we should remap this to work with the web service via HTTP. But this is faster for now
            BsonClassMap.RegisterClassMap<User>(); // do it before you access DB

            services.AddSingleton<IUserRepository, MongoUserRepository>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddTransient<IStringNormaliser, UpperStringNormaliser>();

            services.AddTransient<IResourceOwnerPasswordValidator, PasswordValidator>();

            services.Configure<BCryptPasswordHasherOptions>(options =>
            {
                options.WorkFactor = 10;
                options.EnhancedEntropy = false;
            });

            services.Configure<MongoDbSettings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.Database = Configuration.GetSection("MongoConnection:Database").Value;
            });

            // End of TechDevs Acounts Wireup
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseDeveloperExceptionPage();
            app.UseCors("default");
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }      
    }
}