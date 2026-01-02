using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Products(
            int page = 1,
            int pageSize = 4,
            string sortBy = "name",
            string sortOrder = "asc",
            int? categoryId = null)
        {
            // ОТЛАДКА: логируем параметры
            _logger.LogInformation($"Products action called. Page: {page}, CategoryId: {categoryId}, SortBy: {sortBy}");

            // Проверяем данные в базе
            var totalProducts = _context.Products.Count();
            var totalCategories = _context.Categories.Count();

            _logger.LogInformation($"В базе: {totalProducts} продуктов, {totalCategories} категорий");

            // 1. Оптимизация запросов
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .AsNoTracking();

            // 2. Добавляем фильтрацию по категории
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
                _logger.LogInformation($"Фильтр по категории: {categoryId.Value}");
            }

            // 3. Добавляем сортировку
            query = sortBy.ToLower() switch
            {
                "price" => sortOrder == "asc"
                    ? query.OrderBy(p => p.Price)
                    : query.OrderByDescending(p => p.Price),
                _ => sortOrder == "asc"
                    ? query.OrderBy(p => p.Name)
                    : query.OrderByDescending(p => p.Name),
            };

            // 4. Добавляем пагинацию
            var totalCount = query.Count();
            _logger.LogInformation($"После фильтрации: {totalCount} продуктов");

            var products = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            _logger.LogInformation($"Загружено {products.Count} продуктов для страницы {page}");

            // 5. Создаем ViewModel для передачи данных в представление
            var viewModel = new ProductListViewModel
            {
                Products = products,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                SortBy = sortBy,
                SortOrder = sortOrder,
                CategoryId = categoryId
            };

            // 6. Передаем категории для фильтра
            var categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            ViewBag.Categories = categories;
            _logger.LogInformation($"Передано {categories.Count} категорий в ViewBag");

            // Проверяем, что передается в представление
            _logger.LogInformation($"В ViewModel передано {viewModel.Products?.Count ?? 0} продуктов");
            _logger.LogInformation($"TotalPages: {viewModel.TotalPages}");

            return View(viewModel);
        }
    }
}