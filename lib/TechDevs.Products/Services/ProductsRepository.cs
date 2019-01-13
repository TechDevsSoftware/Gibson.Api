using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.Products
{
    public interface IProductsRepository
    {
        Task<List<Product>> GetProducts();
        Task<Product> GetProduct(string id);
        Task<Product> CreateProduct(Product product);
        Task<Product> UpdateProduct(Product product);
        Task<Product> DeleteProduct(string productId);
    }

    public class ProductsRepository : IProductsRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Product> _products;

        public ProductsRepository(IOptions<MongoDbSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            _database = client.GetDatabase(dbSettings.Value.Database);
            _products = _database.GetCollection<Product>("Products");
        }

        public async Task<Product> GetProduct(string id)
        {
            var result = await _products.FindAsync(x => x.Id == id);
            return await result.FirstOrDefaultAsync();
        }

        public async Task<List<Product>> GetProducts()
        {
            var result = await _products.FindAsync(x => true);
            return await result.ToListAsync();
        }

        public async Task<Product> CreateProduct(Product product)
        {
            await _products.InsertOneAsync(product);
            var result = await GetProduct(product.Id);
            return result;
        }

        public async Task<Product> DeleteProduct(string productId)
        {
            var result = await _products.FindOneAndDeleteAsync(x => x.Id == productId);
            return result;
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            FilterDefinition<Product> filterDef = Builders<Product>.Filter.Eq(x => x.Id, product.Id);
            var updateDef = new UpdateOptions { IsUpsert = false };
            var updateResult = await _products.ReplaceOneAsync(filterDef, product, updateDef);
            if (!updateResult.IsAcknowledged) throw new Exception("Product could not be updated");
            return await GetProduct(product.Id);
        }
    }
}