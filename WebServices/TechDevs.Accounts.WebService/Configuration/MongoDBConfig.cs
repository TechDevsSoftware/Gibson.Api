using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using TechDevs.Shared.Models;

namespace TechDevs.Gibson.WebService
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
                c.AutoMap();
            });

            BsonClassMap.RegisterClassMap<Employee>(c =>
            {
                c.SetDiscriminator("Employee");
                c.SetDiscriminatorIsRequired(true);
            });
        }
    }
}