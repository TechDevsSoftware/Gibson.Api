using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechDevs.Clients;
using TechDevs.Clients.Theme;
using TechDevs.NotificationPreferences;
using TechDevs.Shared.Models;
using TechDevs.Shared.Utils;
using TechDevs.MarketingPreferences;
using TechDevs.Clients.Offers;
using Audit.WebApi;
using TechDevs.Customers;
using TechDevs.Users;
using GraphQL.Types;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Http;
using TechDevs.Employees;
using Gibson.CustomerVehicles;
using Gibson.BookingRequests;
using Gibson.AuthTokens;

namespace TechDevs.Gibson.Api
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

            //services.AddTransient(typeof(ICustomerDataRepository<>), typeof(CustomerDataRepository<>));

            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IAuthUserRepository<Customer>, CustomerRepository>();
            services.AddTransient<IAuthUserRepository<Employee>, EmployeeRepository>();
            services.AddTransient<IAuthUserRepository<AuthUser>, UserRepository>();
            services.AddTransient<ICustomerVehicleRepository, CustomerVehicleRespository>();
            services.AddTransient<IBookingRequestRepository, BookingRequestsRepository>();
            // Services
            services.AddTransient<IUserService<AuthUser>, UserService>();
            services.AddTransient<IUserService<Customer>, CustomerService>();
            services.AddTransient<IUserService<Employee>, EmployeeService>();
            services.AddTransient<IAuthService<Customer>, AuthService<Customer>>();
            services.AddTransient<IAuthService<AuthUser>, AuthService<AuthUser>>();
            services.AddTransient<IAuthService<Employee>, AuthService<Employee>>();
            services.AddTransient<IAuthTokenService, AuthTokenService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IClientThemeService, ClientThemeService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<ICustomerVehicleService, CustomerVehicleService>();
            services.AddTransient<IVehicleDataService, VehicleDataService>();
            services.AddTransient<ICustomerVehicleService, CustomerVehicleService>();

            services.AddTransient<INotificationPreferencesService, NotificationPreferencesService>();
            services.AddTransient<IMarketingPreferencesService, MarketingPreferencesService>();
            services.AddTransient<IBasicOffersService, BasicOffersService>();
            services.AddTransient<IBookingRequestService, BookingRequestService>();

            // Utils
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddTransient<IStringNormaliser, UpperStringNormaliser>();


            // GraphQL Models
            services.AddTransient<ServiceDataModel>();
            services.AddTransient<BookingRequestModel>();
            services.AddTransient<BookingCustomerModel>();
            services.AddTransient<MotCommentModel>();
            services.AddTransient<MotDataModel>();
            services.AddTransient<MotResultModel>();
            services.AddTransient<ClientThemeModel>();
            services.AddTransient<CSSParameterModel>();
            services.AddTransient<BookingRequestModel>();
            services.AddTransient<ClientModel>();
            services.AddTransient<EmployeeModel>();
            services.AddTransient<ClientDataModel>();
            services.AddTransient<BasicOfferModel>();
            services.AddTransient<CustomerModel>();
            services.AddTransient<CustomerVehicleModel>();
            services.AddTransient<MarketingNotificationPreferencesModel>();
            services.AddTransient<CustomerNotificationPreferencesModel>();
            // GraphQL Query
            services.AddTransient<GibsonQuery>();
            // GraphQL Schema
            services.AddTransient<ISchema, GibsonSchema>();
            // GraphQL Dependency Resolver
            services.AddTransient<IDependencyResolver>(c => new FuncDependencyResolver(type => c.GetRequiredService(type)));
            // HttpAccessor used to get access to the request headers in GraphQL queries
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

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
                //.AddUserSecrets<SMTPSettings>()
                .AddUserSecrets<MongoDbSettings>()
                .AddUserSecrets<AppSettings>()
                .AddEnvironmentVariables();

            // Configure
            //services.Configure<SMTPSettings>(Configuration.GetSection(nameof(SMTPSettings)));

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


            // Configure GraphQL
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


            //// Setup the API Audit DB
            //Audit.Core.Configuration
            //.Setup()
            //.UseMongoDB(config => config
            //.ConnectionString(Configuration.GetSection(nameof(MongoDbSettings)).GetValue<string>("ConnectionString"))
            //.Database(Configuration.GetSection(nameof(MongoDbSettings)).GetValue<string>("Database"))
            //.Collection("APIAudit"));

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


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "User Profile API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "Bearer", Enumerable.Empty<string>() },
                });
                //c.OperationFilter<TechDevsClientKeyHeaderFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("default");

            // Add Custom configuration for Audit API

            //SensitiveInformation.Custom();

            //app.Use(async (context, next) =>
            //{  // <----
            //    context.Request.EnableRewind();
            //    await next();
            //});


            //app.UseAuditMiddleware(_ => _
            //.WithEventType("{verb}:{url}")
            //.IncludeHeaders()
            //.IncludeResponseHeaders()
            //.IncludeRequestBody()
            //.IncludeResponseBody());


            // add http for Schema at default url
            app.UseGraphQL<ISchema>("/graphql");

            // use graphql-playground at default url /ui/playground
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions
            {
                Path = "/ui/graphql"
            });

            app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Profile API v1");
            });


        }
    }

    public static class SensitiveInformation
    {
        public static void Custom()
        {
            Audit.Core.Configuration.AddCustomAction(Audit.Core.ActionType.OnEventSaving, action =>
            {
                var mvc = action.Event.GetWebApiAuditAction();//.GetMvcAuditAction();

                if (mvc.ActionName.ToUpper() == "LOGIN")
                {
                    mvc.RequestBody.Value = null;
                    if (mvc.ActionParameters.ContainsKey("request"))
                    {
                        dynamic x = mvc.ActionParameters["request"];
                        mvc.ActionParameters["request"] = RemoveSensitiveData(x);
                    }
                }
            });
        }

        private static object RemoveSensitiveData(object vm)
        {
            return vm;
        }

        private static LoginRequest RemoveSensitiveData(LoginRequest vm)
        {
            vm.Password = "{RemovedSensitiveInformation}";
            return vm;
        }
    }
}