using System;
using Gibson.BookingRequests;
using Gibson.CustomerVehicles;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using TechDevs.Clients;
using TechDevs.Customers;
using TechDevs.Employees;
using TechDevs.Shared.Models;
using TechDevs.Shared.Utils;
using TechDevs.Users;
using TechDevs.Users.GraphQL.Resolvers;

namespace TechDevs.Gibson.Api
{
    public class GibsonQuery : ObjectGraphType<object>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IAuthService<AuthUser> auth;
        private bool Authenticated() => auth.ValidateToken(httpContext.GetAuthToken());

        public GibsonQuery(
            IClientService clientService,
            IEmployeeService employees,
            ICustomerService customers,
            IBookingRequestService bookingRequests,
            IHttpContextAccessor ctx,
            IAuthService<AuthUser> auth,
            ICustomerVehicleService vehicles
            )
        {
            this.httpContext = ctx;
            this.auth = auth;

            var token = ctx.GetAuthToken();
            var clientKey = token.GetClientKey();
            var userId = token.GetUserId();
            var clientId = token.GetClientId();

            Name = "Query";

            Field<ListGraphType<ClientModel>>("clients", resolve: context => Authenticated() ? clientService.GetClients() : throw new Exception("Not authenticated"));
            Field<ListGraphType<EmployeeModel>>("employees", resolve: context => Authenticated() ? employees.GetAllUsers(clientKey) : throw new Exception("Not authenticated"));
            Field<ListGraphType<CustomerModel>>("customers", resolve: context => Authenticated() ? customers.GetAllUsers(clientKey) : throw new Exception("Not authenticated"));
            Field<ListGraphType<BookingRequestModel>>("bookingRequests", resolve: c => Authenticated() ? bookingRequests.GetBookings(clientId) : throw new Exception("Not authenticated"));

            Field<ListGraphType<CustomerVehicleModel>>("myVehicles", resolve: context => Authenticated() ? vehicles.GetCustomerVehicles(userId, clientId) : throw new Exception("Not authenticated"));

            Field<ClientModel>("client", resolve: c => {
                return Authenticated() ? clientService.GetClient(clientId.ToString()) : throw new Exception("Not authenticated");
            });

            Field<CustomerModel>("myProfile", resolve: c => Authenticated() ? customers.GetById(userId.ToString(), clientId.ToString()) : throw new Exception("Not authenticated"));
            Field<CustomerModel>("myCustomerProfile", resolve: c => Authenticated() ? customers.GetById(userId.ToString(), clientId.ToString()) : throw new Exception("Not authenticated"));
            Field<EmployeeModel>("myEmployeeProfile", resolve: c => Authenticated() ? employees.GetByJwtToken(ctx.GetAuthToken()) : throw new Exception("Not authenticated"));

            Field<CustomerModel>("customer", arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "customerId" }), resolve: c =>
            {
                return Authenticated() ? customers.GetById(c.GetArgument<string>("customerId"), clientKey) : null;
            });

            Field<BookingRequestModel>("bookingRequest", arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "bookingId" }), resolve: c =>
            {
                return Authenticated() ? bookingRequests.GetBooking(Guid.Parse(c.GetArgument<string>("bookingId")), clientId) : throw new Exception("Not authenticated");
            });

            Field<ClientModel>("clientByKey", arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "shortKey" }), resolve: c =>
            {
                return Authenticated() ? clientService.GetClientByShortKey(c.GetArgument<string>("shortKey")) : throw new Exception("Not authenticated");
            });

        }
    }

    public class ClientModel : ObjectGraphType<Client>
    {
        private readonly IAuthService<AuthUser> auth;
        private readonly IHttpContextAccessor httpContext;

        private bool Authenticated() => auth.ValidateToken(httpContext.GetAuthToken());

        public ClientModel(IEmployeeService employees, ICustomerService customers, IAuthService<AuthUser> auth, IHttpContextAccessor httpContext)
        {
            this.auth = auth;
            this.httpContext = httpContext;

            Field(c => c.Id);
            Field(x => x.Name);
            Field(x => x.ShortKey);
            Field(x => x.SiteUrl, true);
            Field<ListGraphType<EmployeeModel>>("employees", resolve: context => Authenticated() ? employees.GetAllUsers(context.Source.Id) : throw new Exception("Not authenticated"));
            Field<ListGraphType<CustomerModel>>("customers", resolve: context => Authenticated() ? customers.GetAllUsers(context.Source.Id) : throw new Exception("Not authenticated"));
            Field<ClientDataModel>("clientData", resolve: c => c.Source.ClientData);
            Field<ClientThemeModel>("clientTheme", resolve: c => c.Source.ClientTheme);
        }
    }


    public class PublicClientModel : ObjectGraphType<Client>
    {
        public PublicClientModel()
        {
            // This should only show public safe fields 
            Field(c => c.Name);
            Field(c => c.ShortKey);
            Field(c => c.ClientTheme);
        }
    }

    public class BookingRequestModel : ObjectGraphType<BookingRequest>
    {
        public BookingRequestModel()
        {
            Field(f => f.Id, type: typeof(IdGraphType));
            Field(f => f.ClientId, type: typeof(IdGraphType));
            Field(f => f.MOT).Name("mot");
            Field(f => f.Service);
            Field(f => f.PreferedDate);
            Field(f => f.PreferedTime);
            Field(f => f.Message, true);
            Field(f => f.Confirmed);
            Field(f => f.Cancelled);
            Field(f => f.ConfirmationEmailSent);
            Field<DateTimeGraphType>("RequestDate");
            Field<BookingCustomerModel>("customer", resolve: c => c.Source.Customer);
            Field<CustomerVehicleModel>("vehicle", resolve: c => c.Source.Vehicle);
        }
    }

    public class BookingCustomerModel : ObjectGraphType<BookingCustomer>
    {
        public BookingCustomerModel()
        {
            Field(f => f.Id, type: typeof(IdGraphType));
            Field(f => f.ClientId, type: typeof(IdGraphType));
            Field(x => x.FirstName);
            Field(x => x.LastName);
            Field(x => x.EmailAddress);
            Field(x => x.ContactNumber, true);
        }
    }

    public class ClientThemeModel : ObjectGraphType<ClientTheme>
    {
        public ClientThemeModel()
        {
            Field(x => x.Font, true);
            Field(x => x.LogoPath, true);
            Field<ListGraphType<CSSParameterModel>>("cssParameters", resolve: c => c.Source.CssParameters);
        }
    }

    public class CSSParameterModel : ObjectGraphType<CSSParameter>
    {
        public CSSParameterModel()
        {
            Field(x => x.Key);
            Field(x => x.Value, true);
        }
    }

    public class ClientDataModel : ObjectGraphType<ClientData>
    {
        public ClientDataModel()
        {
            Field<ListGraphType<BasicOfferModel>>("basicOffers", resolve: c => c.Source.BasicOffers);
            //Field<ListGraphType<BookingRequestModel>>("bookingRequests", resolve: c => c.Source.BookingRequests);
        }
    }

    public class BasicOfferModel : ObjectGraphType<BasicOffer>
    {
        public BasicOfferModel()
        {
            Field(b => b.Id);
            Field(b => b.Badge, true);
            Field(b => b.Description, true);
            Field(b => b.Enabled);
            Field(b => b.ImageSrc, true);
            Field(b => b.OfferCode, true);
            Field(b => b.Title, true);
        }
    }

    public class EmployeeModel : ObjectGraphType<Employee>
    {
        public EmployeeModel()
        {
            Field(e => e.Id);
            Field(e => e.FirstName);
            Field(e => e.LastName);
            Field(e => e.JobTitle, true);
            Field(e => e.AgreedToTerms);
            Field(e => e.ContactNumber, true);
            Field(e => e.Disabled);
            Field(e => e.EmailAddress);
            Field(e => e.ProviderId, true);
            Field(e => e.ProviderName, true);
            Field(e => e.Role, true);
            Field(e => e.Username);
        }
    }

    public class CustomerModel : ObjectGraphType<Customer>
    {
        public CustomerModel(IBookingRequestService bookings, ICustomerVehicleService vehicles)
        {
            Field(e => e.Id);
            Field(e => e.FirstName);
            Field(e => e.LastName);
            Field(e => e.AgreedToTerms);
            Field(e => e.ContactNumber, true);
            Field(e => e.Disabled);
            Field(e => e.EmailAddress);
            Field(e => e.ProviderId, true);
            Field(e => e.ProviderName, true);
            Field(e => e.Username);
            Field<ListGraphType<BookingRequestModel>>("bookings", resolve: c => bookings.GetBookingsByCustomer(Guid.Parse(c.Source.Id), Guid.Parse(c.Source.ClientId.Id)));
            Field<ListGraphType<CustomerVehicleModel>>("vehicles", resolve: c => vehicles.GetCustomerVehicles(Guid.Parse(c.Source.Id), Guid.Parse(c.Source.ClientId.Id)));
            Field<MarketingNotificationPreferencesModel>("marketingPreferences", resolve: c => c.Source.CustomerData.MarketingPreferences);
            Field<CustomerNotificationPreferencesModel>("notificationPreferences", resolve: c => c.Source.CustomerData.NotificationPreferences);
        }
    }

    public class CustomerVehicleModel : ObjectGraphType<CustomerVehicle>
    {
        public CustomerVehicleModel()
        {
            Field(b => b.Id, type: typeof(IdGraphType));
            Field(b => b.ClientId, type: typeof(IdGraphType));
            Field(b => b.CustomerId, type: typeof(IdGraphType));
            Field(b => b.Registration);
            Field(b => b.Make);
            Field(b => b.Model);
            Field(b => b.FuelType);
            Field(b => b.Colour);
            Field(b => b.Year);
            Field<MotDataModel>("motData", resolve: c => c.Source.MotData);
        }
    }

    public class MotDataModel : ObjectGraphType<MotData>
    {
        public MotDataModel()
        {
            Field(x => x.MOTExpiryDate, true);
            Field<ListGraphType<MotResultModel>>("motResults", resolve: c => c.Source.MOTResults);
        }
    }

    public class MotResultModel : ObjectGraphType<MotResult>
    {
        public MotResultModel()
        {
            Field(c => c.CompletedDate, true);
            Field(c => c.TestResult, true);
            Field(c => c.ExpiryDate, true);
            Field(c => c.OdometerValue, true);
            Field(c => c.OdometerUnit, true);
            Field(c => c.OdometerResultType, true);
            Field(c => c.MotTestNumber, true);
            Field<ListGraphType<MotCommentModel>>("comments", resolve: c => c.Source.Comments);
        }
    }

    public class MotCommentModel : ObjectGraphType<MotComment>
    {
        public MotCommentModel()
        {
            Field(x => x.Text, true);
            Field(x => x.Type, true);
        }
    }

    public class MarketingNotificationPreferencesModel : ObjectGraphType<MarketingNotificationPreferences>
    {
        public MarketingNotificationPreferencesModel()
        {
            Field(b => b.Email);
            Field(b => b.Phone);
            Field(b => b.Post);
            Field(b => b.SMS);
        }
    }

    public class CustomerNotificationPreferencesModel : ObjectGraphType<CustomerNotificationPreferences>
    {
        public CustomerNotificationPreferencesModel()
        {
            Field(b => b.Email);
            Field(b => b.SMS);
            Field(b => b.PushNotifications);
        }
    }

    public class GibsonSchema : Schema
    {
        public GibsonSchema(GibsonQuery query, IDependencyResolver resolver)
        {
            Query = query;
            DependencyResolver = resolver;
        }
    }
}
