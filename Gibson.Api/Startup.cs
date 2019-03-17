using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gibson.Clients;
using Gibson.Clients.Theme;
using Gibson.Common.Models;
using Gibson.Common.Utils;
using Gibson.Clients.Offers;
using Gibson.Auth;
using Gibson.Auth.Crypto;
using Gibson.Customers.Vehicles;
using Gibson.Auth.Tokens;
using Gibson.Customers.Bookings;
using Gibson.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using TechDevs.Clients;

namespace Gibson.Api
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }
        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            MongoDBConfig.Configure();

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

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("TechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKey")),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Repositories
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<ICustomerVehicleRepository, CustomerVehicleRespository>();
            services.AddTransient<IBookingRequestRepository, BookingRequestsRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            // Services
            services.AddTransient<IAuthTokenService, AuthTokenService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IClientThemeService, ClientThemeService>();
            services.AddTransient<ICustomerVehicleService, CustomerVehicleService>();
            services.AddTransient<IVehicleDataService, VehicleDataService>();
            services.AddTransient<ICustomerVehicleService, CustomerVehicleService>();
            services.AddTransient<IBasicOffersService, BasicOffersService>();
            services.AddTransient<IBookingRequestService, BookingRequestService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAuthTokenService, AuthTokenService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUserRegistrationService, UserRegistrationService>();
            // Utils
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddTransient<IStringNormaliser, UpperStringNormaliser>();
            
            // Configure the password hashing algorithm
            services.Configure<BCryptPasswordHasherOptions>(options =>
            {
                options.WorkFactor = 10;
                options.EnhancedEntropy = false;
            });

            // Combine the secrets with the app config             
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<MongoDbSettings>()
                .AddUserSecrets<AppSettings>()
                .AddEnvironmentVariables();
            
            if (_env.IsEnvironment("IntegrationTesting"))
            {
                services.Configure<MongoDbSettings>(config =>
                {
                    config.ConnectionString = "mongodb://127.0.0.1:27017/";
                    config.Database = "accounts";
                });
            }
            else
            {
                services.Configure<MongoDbSettings>(Configuration.GetSection(nameof(MongoDbSettings)));
            }
            
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("customer", new Info { Title = "Customer API", Version = "v1" });
                c.SwaggerDoc("client", new Info { Title = "Client API", Version = "v1" });
                c.SwaggerDoc("admin", new Info { Title = "Admin API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "Bearer", Enumerable.Empty<string>() },
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("default");
            app.UseMiddleware<ClientValidationMiddleware>();
            app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/customer/swagger.json", "Customer API v1");
                c.SwaggerEndpoint("/swagger/client/swagger.json", "Client API v1");
                c.SwaggerEndpoint("/swagger/admin/swagger.json", "Admin API v1");
                
            });
        }
    }
}