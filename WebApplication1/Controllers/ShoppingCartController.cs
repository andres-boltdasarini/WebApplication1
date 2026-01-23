using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Models;
using ShoppingCartApp.Services;

namespace ShoppingCartApp.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _cartService;
        private readonly IProductRepository _productRepository;

        public ShoppingCartController(
            IShoppingCartService cartService,
            IProductRepository productRepository)
        {
            _cartService = cartService;
            _productRepository = productRepository;
        }

        // Страница со списком товаров
        public IActionResult Index()
        {
            var products = _productRepository.GetAllProducts();
            return View(products);
        }

        // Добавление товара в корзину
        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            _cartService.AddToCart(productId);

            // Можно вернуть JSON для AJAX запросов
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var cart = _cartService.GetCart();
                return Json(new
                {
                    success = true,
                    totalItems = cart.TotalItems
                });
            }

            return RedirectToAction("Index");
        }

        // Просмотр корзины
        public IActionResult ViewCart()
        {
            var cart = _cartService.GetCart();
            return View(cart);
        }

        // Очистка корзины
        [HttpPost]
        public IActionResult ClearCart()
        {
            _cartService.ClearCart();
            return RedirectToAction("ViewCart");
        }

        // Удаление товара из корзины
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            _cartService.RemoveFromCart(productId);
            return RedirectToAction("ViewCart");
        }
    }
}