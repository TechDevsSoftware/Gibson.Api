using System;
using System.Threading.Tasks;
using ServiceStack.Stripe;
using ServiceStack.Stripe.Types;

namespace TechDevs.Billing.Stripe
{
    public interface IBillingService
    {
        Task<IBillingCustomer> CreateCustomer(IBillingService customer);
        Task<IBillingCustomer> GetCustomer(string email);
    }

    public interface IBillingCustomer
    {
        string Email { get; set; }
    }

    public class StripeBillingService  : IBillingService
    {
        StripeCustomer customer;
        StripeGateway gateway;

        public StripeBillingService()
        {
            gateway = new StripeGateway("");
        }

        public async Task<IBillingCustomer> CreateCustomer(IBillingService customer)
        {
            var result = await gateway.PostAsync(new CreateStripeCustomer
            {
                AccountBalance = 10000,
                Card = new StripeCard
                {
                    Name = "Test Card",
                    Number = "4242424242424242",
                    Cvc = "123",
                    ExpMonth = 1,
                    ExpYear = 2015,
                    AddressLine1 = "1 Address Road",
                    AddressLine2 = "12345",
                    AddressZip = "City",
                    AddressState = "NY",
                    AddressCountry = "US",
                },
                Description = "Description",
                Email = "test@email.com",
            });
        }

        public Task<IBillingCustomer> GetCustomer(string email)
        {
            throw new NotImplementedException();
        }
    }
}
