using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProductsController : Controller
    {


        public IActionResult Products(
            int page = 1,
            int pageSize = 20,
            string sortBy = "name",
            string sortOrder = "asc",
            int? categoryId = null)
        {
            // 1. Оптимизация запросов: убираем N+1 проблему
            var query = _context.Products
                .Include(p => p.Category) // Используем Include для загрузки связанных данных
                .Include(p => p.Images)   // Загружаем изображения одним запросом
                .AsNoTracking(); // Оптимизация для чтения данных

            // 2. Добавляем фильтрацию по категории
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
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
            var totalCount = query.Count(); // Получаем общее количество для пагинации

            var products = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

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

            // 6. Передаем категории для фильтра (если нужно в выпадающем списке)
            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            return View(viewModel);
        }
    }
    }
