using System.IO;
using System.Security.Claims;
using System.Text;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TechDevs.Clients;
using TechDevs.Employees;
using TechDevs.Mail;
using TechDevs.Shared.Models;
using TechDevs.Shared.Utils;
using TechDevs.Users;
using TechDevs.Customers;

namespace TechDevs.Gibson.GraphQLApi
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
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

            // HttpAccessor used to get access to the request headers in GraphQL queries
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            // Gibson Services
            services.AddTransient<IAuthUserRepository<Employee>, EmployeeRepository>();
            services.AddTransient<IAuthUserRepository<Customer>, CustomerRepository>();
            services.AddTransient<IAuthUserService<Employee>, EmployeeService>();
            services.AddTransient<IAuthUserService<Customer>, CustomerService>();
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<EmployeeService, EmployeeService>();
            services.AddTransient<CustomerService, CustomerService>();

            // GraphQL Models
            services.AddTransient<ClientModel>();
            services.AddTransient<EmployeeModel>();
            services.AddTransient<ClientDataModel>();
            services.AddTransient<BasicOfferModel>();
            services.AddTransient<CustomerModel>();
            services.AddTransient<CustomerDataModel>();
            services.AddTransient<CustomerVehicleModel>();
            services.AddTransient<MarketingNotificationPreferencesModel>();
            services.AddTransient<CustomerNotificationPreferencesModel>();
            // GraphQL Query
            services.AddTransient<GibsonQuery>();
            // GraphQL Schema
            services.AddTransient<ISchema, GibsonSchema>();
            // GraphQL Dependency Resolver
            services.AddTransient<IDependencyResolver>(c => new FuncDependencyResolver(type => c.GetRequiredService(type)));

            // Utils
            services.AddTransient<IPasswordHasher, BCryptPasswordHasher>();
            services.AddTransient<IStringNormaliser, UpperStringNormaliser>();
            services.AddTransient<IEmailer, DotNetEmailer>();

            // Configure the passowrd hashing algorithm
            services.Configure<BCryptPasswordHasherOptions>(options =>
            {
                options.WorkFactor = 10;
                options.EnhancedEntropy = false;
            });

            // Combine the secrets with the app config             
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<SMTPSettings>()
                .AddUserSecrets<MongoDbSettings>()
                .AddUserSecrets<AppSettings>()
                .AddEnvironmentVariables();


            // Configure
            services.Configure<SMTPSettings>(Configuration.GetSection(nameof(SMTPSettings)));

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

            if (_env.IsDevelopment() || _env.IsEnvironment("IntegrationTesting"))
            {
                services.Configure<AppSettings>(config =>
                {
                    config.InvitationSiteRoot = "http://localhost:4200";
                });
            }
            else
            {
                services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            }

            services.AddGraphQL(_ =>
            {
                _.EnableMetrics = true;
                _.ExposeExceptions = true;
            })
            .AddUserContextBuilder(httpContext => new GraphQLUserContext
            {
                User = httpContext.User,
                Headers = httpContext.Request.Headers
            });

            services.AddMvc(mvc =>
            {
                //mvc.AddAuditFilter(config => config
                    // .LogAllActions()
                    //.WithEventType("{verb}.{controller}.{action}")
                    //.IncludeHeaders(ctx => !ctx.ModelState.IsValid)
                    //.IncludeRequestBody()
                    //.IncludeModelState()
                    //.IncludeResponseBody());
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

            // add http for Schema at default url
            app.UseGraphQL<ISchema>("/graphql");

            // use graphql-playground at default url /ui/playground
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions
            {
                Path = "/ui/playground"
            });

            app.UseAuthentication();
            app.UseMvc();
        }
    }

    public class GraphQLUserContext
    {
        public ClaimsPrincipal User { get; set; }
        public IHeaderDictionary Headers { get; set; }
    }
}
