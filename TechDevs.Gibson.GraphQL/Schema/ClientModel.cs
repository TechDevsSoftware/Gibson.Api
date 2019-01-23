using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using System.Linq;
using TechDevs.Clients;
using TechDevs.Customers;
using TechDevs.Employees;
using TechDevs.Shared.Models;
using TechDevs.Users;

namespace TechDevs.Gibson.GraphQLApi
{
    public class ClientModel : ObjectGraphType<Client>
    {
        public ClientModel(EmployeeService employees, CustomerService customers)
        {
            Field(c => c.Id);
            Field(x => x.Name);
            Field(x => x.ShortKey);
            Field(x => x.SiteUrl);
            Field<ListGraphType<EmployeeModel>>("employees", resolve: context => employees.GetAllUsers(context.Source.Id));
            Field<ListGraphType<CustomerModel>>("customers", resolve: context => customers.GetAllUsers(context.Source.Id));
            Field<ClientDataModel>("clientData", resolve: c => c.Source.ClientData);
        }
    }

    public class ClientDataModel : ObjectGraphType<ClientData>
    {
        public ClientDataModel()
        {
            Field<ListGraphType<BasicOfferModel>>("basicoffers", resolve: c => c.Source.BasicOffers);
        }
    }

    public class BasicOfferModel : ObjectGraphType<BasicOffer>
    {
        public BasicOfferModel()
        {
            Field(b => b.Id);
            Field(b => b.Badge);
            Field(b => b.Description);
            Field(b => b.Enabled);
            Field(b => b.ImageSrc);
            Field(b => b.OfferCode);
            Field(b => b.Title);
        }
    }

    public class EmployeeModel : ObjectGraphType<Employee>
    {
        public EmployeeModel()
        {
            Field(e => e.Id);
            Field(e => e.FirstName);
            Field(e => e.LastName);
            Field(e => e.JobTitle);
            Field(e => e.AgreedToTerms);
            Field(e => e.ContactNumber);
            Field(e => e.Disabled);
            Field(e => e.EmailAddress);
            Field(e => e.ProviderId);
            Field(e => e.ProviderName);
            Field(e => e.Role);
            Field(e => e.Username);
        }
    }

    public class CustomerModel : ObjectGraphType<Customer>
    {
        public CustomerModel()
        {
            Field(e => e.Id);
            Field(e => e.FirstName);
            Field(e => e.LastName);
            Field(e => e.AgreedToTerms);
            Field(e => e.ContactNumber);
            Field(e => e.Disabled);
            Field(e => e.EmailAddress);
            Field(e => e.ProviderId);
            Field(e => e.ProviderName);
            Field(e => e.Username);
            Field<CustomerDataModel>("customerData", resolve: c => c.Source.CustomerData);
        }
    }

    public class CustomerDataModel : ObjectGraphType<CustomerData>
    {
        public CustomerDataModel()
        {
            Field<ListGraphType<CustomerVehicleModel>>("myVehicles", resolve: c => c.Source.MyVehicles);
            Field<MarketingNotificationPreferencesModel>("marketingPreferences", resolve: c => c.Source.MarketingPreferences);
            Field<CustomerNotificationPreferencesModel>("notificationPreferences", resolve: c => c.Source.NotificationPreferences);
        }
    }

    public class CustomerVehicleModel : ObjectGraphType<CustomerVehicle>
    {
        public CustomerVehicleModel()
        {
            Field(b => b.Registration);
            Field(b => b.Make);
            Field(b => b.Model);
            Field(b => b.FuelType);
            Field(b => b.Colour);
            Field(b => b.Year);
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
            Field(b => b.MotPush);
            Field(b => b.MotEmail);
            Field(b => b.ServicePush);
            Field(b => b.ServiceEmail);
        }
    }



    public class GibsonQuery : ObjectGraphType<object>
    {

        public GibsonQuery(IClientService clients, IAuthUserService<Employee> employees, IAuthUserService<Customer> customers, IHttpContextAccessor httpContext)
        {
            var clientKey = httpContext.GetClientKey();

            //if (string.IsNullOrEmpty(clientKey)) throw new System.Exception("ClientKey is missing");


            Name = "Query";
            Field<ListGraphType<ClientModel>>("clients", resolve: context => clients.GetClients());
            Field<ListGraphType<EmployeeModel>>("employees", resolve: context => employees.GetAllUsers(clientKey));
            Field<ListGraphType<CustomerModel>>("customers", resolve: context => customers.GetAllUsers(clientKey));

            Field<ClientModel>(
                 "clientByKey",
                 arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "shortKey" }),
                 resolve: context =>
                 {
                     var shortKey = context.GetArgument<string>("shortKey");
                     return clients.GetClientByShortKey(shortKey);
                 });
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

    public static class GraphQLHelpers
    {
        public static string GetClientKey(this IHttpContextAccessor httpContext)
        {
            httpContext.HttpContext.Request.Headers.TryGetValue("TechDevs-ClientKey", out var keys);
            var clientKey = keys.FirstOrDefault();
            if (string.IsNullOrEmpty(clientKey)) return null;
            return clientKey;
        }
    }
}
