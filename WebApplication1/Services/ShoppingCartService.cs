using ShoppingCartApp.Models;
using System.Collections.Concurrent;

namespace ShoppingCartApp.Services
{
    public interface IShoppingCartService
    {
        void AddToCart(int productId);
        void RemoveFromCart(int productId);
        void ClearCart();
        CartViewModel GetCart();
    }

    public class ShoppingCartService : IShoppingCartService
    {
        // В реальном приложении здесь будет база данных или распределенное хранилище
        private readonly ConcurrentDictionary<int, int> _cartItems = new();
        private readonly IProductRepository _productRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShoppingCartService(
            IProductRepository productRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _productRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;

            // Инициализация корзины из сессии (если используем сессии)
            InitializeCartFromSession();
        }

        private void InitializeCartFromSession()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var cartData = session.GetString("ShoppingCart");
                if (!string.IsNullOrEmpty(cartData))
                {
                    var items = System.Text.Json.JsonSerializer
                        .Deserialize<Dictionary<int, int>>(cartData);
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            _cartItems[item.Key] = item.Value;
                        }
                    }
                }
            }
        }

        private void SaveCartToSession()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var cartData = System.Text.Json.JsonSerializer
                    .Serialize(_cartItems.ToDictionary(x => x.Key, x => x.Value));
                session.SetString("ShoppingCart", cartData);
            }
        }

        public void AddToCart(int productId)
        {
            if (_cartItems.ContainsKey(productId))
            {
                _cartItems[productId]++;
            }
            else
            {
                _cartItems[productId] = 1;
            }

            SaveCartToSession();
        }

        public void RemoveFromCart(int productId)
        {
            if (_cartItems.ContainsKey(productId))
            {
                if (_cartItems[productId] > 1)
                {
                    _cartItems[productId]--;
                }
                else
                {
                    _cartItems.TryRemove(productId, out _);
                }

                SaveCartToSession();
            }
        }

        public void ClearCart()
        {
            _cartItems.Clear();
            SaveCartToSession();
        }

        public CartViewModel GetCart()
        {
            var cartViewModel = new CartViewModel();

            foreach (var item in _cartItems)
            {
                var product = _productRepository.GetProductById(item.Key);
                if (product != null)
                {
                    cartViewModel.Items.Add(new CartItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Price = product.Price,
                        Quantity = item.Value
                    });
                }
            }

            return cartViewModel;
        }
    }
}