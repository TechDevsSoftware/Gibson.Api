using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Gibson.Common.Models;

namespace Gibson.Api
{
    public static class MongoDBConfig
    {
        public static void Configure()
        {
            try
            {
                // Create a convention that all models are set with the IgnoreExtraElements flag by default
                var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
                var pack = new ConventionPack { new NamedIdMemberConvention("id") };
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
            catch (System.Exception)
            {

            }
        }
    }
}