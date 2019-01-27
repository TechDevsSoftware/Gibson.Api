﻿using System;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using TechDevs.Clients;
using TechDevs.Customers;
using TechDevs.Employees;
using TechDevs.Shared.Models;
using TechDevs.Users;
using TechDevs.Users.GraphQL.Resolvers;

namespace TechDevs.Gibson.Api
{
    public class GibsonQuery : ObjectGraphType<object>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IAuthService<AuthUser> auth;
        private bool Authenticated() => auth.ValidateToken(httpContext.GetAuthToken(), httpContext.GetClientKey());

        public GibsonQuery(ClientResolver clients, IClientService clientService, IEmployeeService employees, ICustomerService customers, IHttpContextAccessor httpContext, IAuthService<AuthUser> auth)
        {
            this.httpContext = httpContext;
            this.auth = auth;

            var clientKey = httpContext.GetClientKey(); 

            Name = "Query";

            Field<ListGraphType<ClientModel>>("clients", resolve: context => Authenticated() ? clients.FindAll() : throw new Exception("Not authenticated"));
            Field<ListGraphType<EmployeeModel>>("employees", resolve: context => Authenticated() ? employees.GetAllUsers(clientKey) : throw new Exception("Not authenticated"));
            Field<ListGraphType<CustomerModel>>("customers", resolve: context => Authenticated() ? customers.GetAllUsers(clientKey) : throw new Exception("Not authenticated"));

            Field<ClientModel>(
                 "clientByKey",
                 arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "shortKey" }),
                 resolve: context =>
                 {
                     if (Authenticated())
                     {
                         var shortKey = context.GetArgument<string>("shortKey");
                         return clients.FindOneByKey(shortKey);
                     }
                     return null;
                 });

            Field<ClientModel>("client", resolve: c => clientService.GetClientByShortKey(httpContext.GetClientKey()));
            Field<CustomerModel>("myProfile", resolve: c => Authenticated() ? customers.GetByJwtToken(httpContext.GetAuthToken(), httpContext.GetClientKey()) : throw new Exception("Not authenticated"));
        }
    }

    public class ClientModel : ObjectGraphType<Client>
    {
        private readonly IAuthService<AuthUser> auth;
        private readonly IHttpContextAccessor httpContext;

        private bool Authenticated() => auth.ValidateToken(httpContext.GetAuthToken(), httpContext.GetClientKey());

        public ClientModel(IEmployeeService employees, ICustomerService customers, IAuthService<AuthUser> auth, IHttpContextAccessor httpContext)
        {
            this.auth = auth;
            this.httpContext = httpContext;

            Field(c => c.Id);
            Field(x => x.Name);
            Field(x => x.ShortKey);
            Field(x => x.SiteUrl, true);
            Field<ListGraphType<EmployeeModel>>("employees", resolve: context => Authenticated() ? employees.GetAllUsers(context.Source.Id) : null);
            Field<ListGraphType<CustomerModel>>("customers", resolve: context => Authenticated() ? customers.GetAllUsers(context.Source.Id) : null);
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
        public BookingRequestModel(ICustomerService customers, IHttpContextAccessor httpContext)
        {
            //Field(f => f.Id);
            Field(f => f.VehicleRegistration);
            Field(f => f.MOT);
            Field(f => f.Service);
            Field(f => f.PreferedDate);
            Field(f => f.PreferedTime);
            Field(f => f.Message, true);
            Field(f => f.Confirmed);
            Field(f => f.Cancelled);
            Field(f => f.ConfirmationEmailSent);
            Field<CustomerModel>("customer", resolve: c => customers.GetById(c.Source.CustomerId.Id, httpContext.GetClientKey()));
        }
    }

    public class ClientThemeModel : ObjectGraphType<ClientTheme>
    {
        public ClientThemeModel()
        {
            Field(x => x.Font, true);
            Field(x => x.LogoPath, true);
            Field<ListGraphType<CSSParameterModel>>("cssParameters", resolve: c => c.Source.Parameters);
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
            Field<ListGraphType<BookingRequestModel>>("bookingRequests", resolve: c => c.Source.BookingRequests);
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
        public CustomerModel()
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
            Field(b => b.MOTExpiryDate, true).Name("motExpiryDate");
            Field<ListGraphType<MotResultModel>>("results", resolve: x => x.Source.MOTResults);
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
            Field(b => b.MotPush);
            Field(b => b.MotEmail);
            Field(b => b.ServicePush);
            Field(b => b.ServiceEmail);
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