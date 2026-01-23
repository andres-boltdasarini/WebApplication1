using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public interface IProductRepository
    {
        Product GetProductById(int id);
        List<Product> GetAllProducts();
    }

    public class ProductRepository : IProductRepository
    {
        // Временные данные для примера
        private readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Ноутбук", Price = 50000, Description = "Мощный ноутбук" },
            new Product { Id = 2, Name = "Смартфон", Price = 30000, Description = "Современный смартфон" },
            new Product { Id = 3, Name = "Наушники", Price = 5000, Description = "Беспроводные наушники" },
            new Product { Id = 4, Name = "Мышь", Price = 1500, Description = "Игровая мышь" },
            new Product { Id = 5, Name = "Клавиатура", Price = 3500, Description = "Механическая клавиатура" }
        };

        public Product GetProductById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public List<Product> GetAllProducts()
        {
            return _products;
        }
    }
}