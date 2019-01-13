using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Products;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts.Controllers.Products
{
    [Route("api/v1/products")]
    public class ProductsController : Controller
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet]
        [Produces(typeof(List<Product>))]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productsService.GetProducts();
            return new OkObjectResult(products);
        }

        [HttpGet("{productId}")]
        [Produces(typeof(Product))]
        public async Task<IActionResult> GetProduct(string productId)
        {
            var product = await _productsService.GetProduct(productId);
            return new OkObjectResult(product);
        }

        [HttpPost]
        [Produces(typeof(Product))]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            var result = await _productsService.CreateProduct(product);
            return new OkObjectResult(result);
        }

        [HttpPut("{productId}")]
        [Produces(typeof(Product))]
        public async Task<IActionResult> UpdateProduct(string productId, [FromBody] Product product)
        {
            var result = await _productsService.UpdateProduct(product);
            return new OkObjectResult(result);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(string productId)
        {
            var result = await _productsService.DeleteProduct(productId);
            return new OkObjectResult(result);
        }

    }
}
