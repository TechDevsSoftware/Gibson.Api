using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TechDevs.Accounts.WebService
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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

            services.AddMvcCore()
               .AddAuthorization()
               .AddJsonFormatters();

            var identityServer = (_env.IsDevelopment()) ? "http://localhost:5000" : "https://techdevs-identityserver.azurewebsites.net";

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    //options.Audience = "http://localhost:4200";
                    //options.Authority = "http://localhost:5101";
                    options.RequireHttpsMetadata = false;
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("techdevstechdevstechdevstechdevstechdevstechdevstechdevstechdevstechdevstechdevs")),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true
                    };

                });

            BsonClassMap.RegisterClassMap<User>(); // do it before you access DB
            BsonClassMap.RegisterClassMap<UserData>(); // do it before you access DB
            BsonClassMap.RegisterClassMap<UserVehicle>(); // do it before you access DB

            services.AddSingleton<IUserRepository, MongoUserRepository>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IAuthTokenService, AuthTokenService>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddTransient<IStringNormaliser, UpperStringNormaliser>();
            services.AddTransient<IMyVehicleService, MyVehicleService>();

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

            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "User Profile API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("default");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Profile API v1");
            });
        }
    }
}