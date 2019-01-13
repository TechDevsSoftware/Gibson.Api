using System.Collections.Generic;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.Products
{
    public interface IProductsService
    {
        Task<List<Product>> GetProducts();
        Task<Product> GetProduct(string id);
        Task<Product> CreateProduct(Product product);
        Task<Product> UpdateProduct(Product product);
        Task<Product> DeleteProduct(string productId);
    }

    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _products;

        public ProductsService(IProductsRepository products)
        {
            _products = products;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            return await _products.CreateProduct(product);
        }

        public async Task<Product> DeleteProduct(string productId)
        {
            return await _products.DeleteProduct(productId);
        }

        public async Task<Product> GetProduct(string id)
        {
            return await _products.GetProduct(id);
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _products.GetProducts();
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            return await _products.UpdateProduct(product);
        }
    }
}