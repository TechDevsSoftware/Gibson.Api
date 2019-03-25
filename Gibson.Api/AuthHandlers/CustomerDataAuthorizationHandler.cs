using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Gibson.Clients;
using Gibson.Common.Enums;
using Gibson.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Gibson.Api.AuthHandlers
{
    public class AuthorizedToTechDevsData : IAuthorizationRequirement
    {
    }

    public class AuthorizedToClientData : IAuthorizationRequirement
    {
    }

    public class AuthorizedToCustomerData : IAuthorizationRequirement
    {
    }

    public class TechDevsDataAuthorizationHandler : AuthorizationHandler<AuthorizedToTechDevsData>
    {
        private readonly IClientService _clientService;

        public TechDevsDataAuthorizationHandler(IClientService clientService)
        {
            _clientService = clientService;
        }
        
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AuthorizedToTechDevsData requirement)
        {
            var usersClientId = context.User.ClientId();
            var client = await _clientService.GetClient(usersClientId.ToString());
            if(client ==  null) context.Fail();
            if(client.ShortKey != "techdevs") context.Fail();
            context.Succeed(requirement);
        }
    }

    public class ClientDataAuthorizationHandler : AuthorizationHandler<AuthorizedToClientData>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AuthorizedToClientData requirement)
        {
            context.Succeed(requirement);
        }
    }

    public class CustomerDataAuthorizationHandler : AuthorizationHandler<AuthorizedToCustomerData>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public CustomerDataAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AuthorizedToCustomerData requirement)
        {
            try
            {
                // Get the UserType
                var typeVal = context.User.FindFirstValue("Gibson-UserType");
                var userTypeSuccessful = GibsonUserType.TryParse(typeVal, out GibsonUserType userType);
                if (!userTypeSuccessful) context.Fail();
                // Get the UserId
                var userId = context.User.UserId();
                if (userId == Guid.Empty) context.Fail();
                // Get the ClientId
                var clientIdVal = context.User.FindFirstValue("Gibson-ClientId");
                var clientIdSuccessful = Guid.TryParse(clientIdVal, out var clientId);
                if (!clientIdSuccessful) context.Fail();
                // Get the UserId from the Route
                var routeUserId = _httpContextAccessor.HttpContext.GetRouteValue("customerId")?.ToString() ??
                                  _httpContextAccessor.HttpContext.GetRouteValue("userId")?.ToString();
                if(routeUserId == null) context.Fail();
                if (!Guid.TryParse(routeUserId, out var customerId))
                {
                    context.Fail();
                    return;
                }

                switch (userType)
                {
                    case GibsonUserType.Customer:
                    {
                        // Make sure that the CustomerId is matching the entity CustomerId
                        if (userId == customerId) context.Succeed(requirement);
                        break;
                    }
                    case GibsonUserType.ClientEmployee:
                        // Make sure that the employee is part of the entity group
                        var customer = await _userService.FindById(customerId, clientId);
                        if (customer == null) context.Fail();
                        // Check that the customers clientId is equal to the employees clientId
                        if (customer?.ClientId == clientId) context.Succeed(requirement);
                        break;
                    case GibsonUserType.TechDevsEmployee:
                        // Make sure that the TechDevs employee Id is valid
                        var techDevsEmployee = await _userService.FindById(userId, clientId);
                        if (techDevsEmployee == null) context.Fail();
                        context.Succeed(requirement);
                        break;
                    case GibsonUserType.NotSet:
                        context.Fail();
                        break;
                    default:
                        context.Fail();
                        break;
                }
            }
            catch (Exception)
            {
                context.Fail();
            }
        }
    }
}