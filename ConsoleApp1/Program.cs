using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDbGenericRepository;
using MongoDbGenericRepository.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var custRepo = new CustomerRepo("mongodb://apiuser:Password2@ds020218.mlab.com:20218/accounts", "accounts");
            
            var result = custRepo.GetAll<AuthUser>(x => true);

            result.ForEach(x => Console.WriteLine(x.Username));

            //var repo = new Repo();
            ////repo.GetClients();
            ////repo.GetAuthUsers();

            //repo.CreateUser("NewUser", Guid.Parse("C5F029C0-0090-4417-854B-6A471F8DD1CB"));
            //repo.CreateCustomer("NewCustomer", Guid.Parse("C5F029C0-0090-4417-854B-6A471F8DD1CB"));
            //repo.CreateEmployee("NewEmployee", Guid.Parse("C5F029C0-0090-4417-854B-6A471F8DD1CB"));

            //repo.GetClients();
            //repo.GetAuthUsers();

            Console.ReadKey();
        }
    }

    [BsonIgnoreExtraElements]
    public class Client
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BusinessTitle { get; set; }
        //ClientTheme Theme { get; set; }
        public List<MongoDBRef> Customers { get; set; }
        public List<MongoDBRef> Employees { get; set; }
    }

    public class ClientDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BusinessTitle { get; set; }
        //ClientTheme Theme { get; set; }
        public List<Customer> Customers { get; set; }
        public List<Employee> Employees { get; set; }

        public ClientDTO(Client client)
        {
            Id = client.Id;
            Name = client.Name;
            BusinessTitle = client.BusinessTitle;
        }
    }

    [BsonIgnoreExtraElements]
    [BsonKnownTypes(typeof(Customer), typeof(Employee))]
    public class AuthUser : IDocument
    {
        [BsonId]
        public Guid Id { get; set; }
        public int Version { get; set; }
        public MongoDBRef ClientId { get; set; }
        public string Username { get; set; }
    }

    public class Customer : AuthUser
    {
        public CustomerData CustomerData { get; set; }
    }

    public class Employee : AuthUser
    {
        public EmployeeData EmployeeData { get; set; }
    }

    public class EmployeeData
    {
        public string EmployeeId { get; set; }
    }

    public class CustomerData
    {
        public string CustomerId { get; set; }
    }

    public class CustomerRepo : BaseMongoRepository
    {
        public CustomerRepo(string connectionString, string databaseName): base (connectionString, databaseName)
        {
        }
    }

    public class Repo
    {
        readonly IMongoDatabase _database;
        readonly IMongoCollection<Client> _clients;
        readonly IMongoCollection<AuthUser> _authUsers;

        public Repo()
        {
            BsonClassMap.RegisterClassMap<AuthUser>(); // do it before you access DB
            BsonClassMap.RegisterClassMap<Client>(); // do it before you access DB

            var client = new MongoClient("mongodb://apiuser:Password2@ds020218.mlab.com:20218/accounts");
            if (client != null) _database = client.GetDatabase("accounts");
            if (_database != null) _clients = _database.GetCollection<Client>("Clients");
            if (_database != null) _authUsers = _database.GetCollection<AuthUser>("AuthUsers");
        }

        public void GetClients()
        {
            var clients = _clients.Find(_ => true).ToList();
            Console.WriteLine("Clients");
            Console.WriteLine(JsonConvert.SerializeObject(clients));
            Console.WriteLine("");
        }

        public void GetAuthUsers()
        {
            var users = _authUsers.Find(_ => true).ToList();
            Console.WriteLine("AuthUsers");
            Console.WriteLine(JsonConvert.SerializeObject(users));
            Console.WriteLine("");
            users.ForEach(u => Console.WriteLine(u.Username));
        }

        public List<Customer> GetCustomers(string clientId)
        {
            var customers = _authUsers.OfType<Customer>();
            var filter = Builders<Customer>.Filter.Eq(x => x.ClientId.Id, clientId);
            var result = customers.Find(x => true).ToList();
            return result;
        }

        public Client GetClient(Guid clientId)
        {
            var filter = Builders<Client>.Filter.Eq(x => x.Id, clientId);
            var client = _clients.Find(filter).FirstOrDefault();
            return client;
            //var clientDTO = new ClientDTO(client);
            //clientDTO.Customers = ResolveListAuthUsers(client.Customers);
            //clientDTO.Employees = ResolveListAuthUsers(client.Employees);
            //return clientDTO;
        }

        public void CreateCustomer(string name, Guid clientId)
        {
            var newCust = new Customer
            {
                Id = Guid.NewGuid(),
                Username = name,
                ClientId = new MongoDBRef("Clients", clientId)
            };

            _authUsers.InsertOne(newCust);

            var filter = Builders<Client>.Filter.Eq(c => c.Id, clientId);
            var clientUpdate = Builders<Client>.Update.Push(e => e.Customers, new MongoDBRef("AuthUsers", newCust.Id));
            _clients.FindOneAndUpdate(filter, clientUpdate);
        }

        public void CreateEmployee(string name, Guid clientId)
        {
            var newEmp = new Employee
            {
                Id = Guid.NewGuid(),
                Username = name,
                ClientId = new MongoDBRef("Clients", clientId),
                EmployeeData = new EmployeeData
                {
                    EmployeeId = "159407"
                }
            };

            _authUsers.InsertOne(newEmp);

            var filter = Builders<Client>.Filter.Eq(c => c.Id, clientId);
            var clientUpdate = Builders<Client>.Update.Push(e => e.Customers, new MongoDBRef("AuthUsers", newEmp.Id));
            _clients.FindOneAndUpdate(filter, clientUpdate);
        }

        public List<AuthUser> ResolveListAuthUsers(List<MongoDBRef> refs)
        {
            var filter = Builders<AuthUser>.Filter.In(x => x.Id, refs.Select(rf => rf.Id));
            var users = _authUsers.Find(filter).ToList();
            return users;
        }

        public void CreateUser(string username, Guid clientId)
        {
            var newUser = new AuthUser
            {
                Id = Guid.NewGuid(),
                Username = username,
                ClientId = new MongoDBRef("Clients", clientId)
            };

            _authUsers.InsertOne(newUser);

            var filter = Builders<Client>.Filter.Eq(c => c.Id, clientId);
            var clientUpdate = Builders<Client>.Update.Push(e => e.Customers, new MongoDBRef("AuthUsers", newUser.Id));
            _clients.FindOneAndUpdate(filter, clientUpdate);
        }
    }
}
