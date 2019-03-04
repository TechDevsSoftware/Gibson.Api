using System;
using Mongo2Go;
using MongoDB.Driver;

namespace Gibson.Shared.Repositories.Tests
{
    public class DatabaseTestFixture : IDisposable
    {
        public MongoDbRunner Db { get; private set; }
        public IMongoDatabase database;

        public DatabaseTestFixture()
        {
            Db = MongoDbRunner.Start();
            var client = new MongoClient(Db.ConnectionString);
            database = client.GetDatabase("Testing");
        }

        public void Dispose()
        {
            database.DropCollection("DummyCollection");
            Db.Dispose();
        }
    }
}
