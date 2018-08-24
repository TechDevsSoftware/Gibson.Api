namespace TechDevs.Accounts
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }

    public class DBRef
    {
        public string Id { get; set; }
    }
}
