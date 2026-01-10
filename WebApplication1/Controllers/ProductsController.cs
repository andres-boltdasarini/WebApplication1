using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize] // Требует авторизации для всех действий
    public class ProductsController : Controller
    {
        // Временное хранилище продуктов (вместо БД)
        private static List<Product> _products = new List<Product>();
        private static int _nextId = 1;

        // Статический список категорий
        private static readonly List<string> _categories = new List<string>
        {
            "Электроника",
            "Одежда",
            "Продукты питания",
            "Книги",
            "Спорт"
        };

        // GET: Products/Index
        public IActionResult Index()
        {
            ViewData["Title"] = "Список продуктов";
            // Убедитесь, что возвращаете _products (не null)
            return View(_products);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")] // Только админ может создавать
        public IActionResult Create()
        {
            ViewData["Title"] = "Создание продукта";
            ViewData["Categories"] = new SelectList(_categories);
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.Id = _nextId++;
                _products.Add(product);
                TempData["SuccessMessage"] = "Продукт успешно создан!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Title"] = "Создание продукта";
            ViewData["Categories"] = new SelectList(_categories);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")] // Только админ может редактировать
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Редактирование продукта";
            ViewData["Categories"] = new SelectList(_categories);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingProduct = _products.FirstOrDefault(p => p.Id == id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Category = product.Category;

                TempData["SuccessMessage"] = "Продукт успешно обновлен!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Title"] = "Редактирование продукта";
            ViewData["Categories"] = new SelectList(_categories);
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // Только админ может удалять
        public IActionResult Delete(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            _products.Remove(product);
            TempData["SuccessMessage"] = "Продукт успешно удален!";
            return RedirectToAction(nameof(Index));
        }
    }
}