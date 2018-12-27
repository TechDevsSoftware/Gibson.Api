using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts.WebService
{
    public static class MongoDBConfig
    {
        public static void Configure()
        {
            // Create a convention that all models are set with the IgnotExtraElements flag by default
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);

            BsonClassMap.RegisterClassMap<Customer>(c =>
            {
                c.SetDiscriminator("Customer");
                c.SetDiscriminatorIsRequired(true);
            });

            // BsonClassMap.RegisterClassMap<CustomerData>(c =>
            // {
            //     c.AutoMap();
            //     c.SetIgnoreExtraElements(true);
            // });
            // BsonClassMap.RegisterClassMap<CustomerVehicle>(c =>
            // {
            //     c.AutoMap();
            //     c.SetIgnoreExtraElements(true);
            // });
            // BsonClassMap.RegisterClassMap<MotResult>(c =>
            // {
            //     c.AutoMap();
            //     c.SetIgnoreExtraElements(true);
            // });
            // BsonClassMap.RegisterClassMap<MotComment>(c =>
            // {
            //     c.AutoMap();
            //     c.SetIgnoreExtraElements(true);
            // });

            BsonClassMap.RegisterClassMap<Employee>(c =>
            {
                c.SetDiscriminator("Employee");
                c.SetDiscriminatorIsRequired(true);
            });

            // BsonClassMap.RegisterClassMap<EmployeeData>(c =>
            // {
            //     c.AutoMap();
            //     c.SetIgnoreExtraElements(true);
            // });

            // BsonClassMap.RegisterClassMap<Client>(c =>
            // {
            //     c.SetDiscriminator("Client");
            //     c.SetDiscriminatorIsRequired(true);
            // });

            // BsonClassMap.RegisterClassMap<EmployeeProfile>(c =>
            // {
            //     c.AutoMap();
            //     c.SetIgnoreExtraElements(true);
            // });
            // BsonClassMap.RegisterClassMap<CustomerProfile>(c =>
            // {
            //     c.AutoMap();
            //     c.SetIgnoreExtraElements(true);
            // });
            // BsonClassMap.RegisterClassMap<ClientTheme>(c =>
            // {
            //     c.AutoMap();
            //     c.SetIgnoreExtraElements(true);
            // });
            // BsonClassMap.RegisterClassMap<CSSParameter>(c =>
            // {
            //     c.AutoMap();
            //     c.SetIgnoreExtraElements(true);
            // });


        }
    }
}