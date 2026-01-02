using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавляем логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Добавляем сервисы в контейнер
builder.Services.AddControllersWithViews();

// Конфигурация PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableSensitiveDataLogging(true);
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

var app = builder.Build();

// Настраиваем конвейер HTTP-запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// СОЗДАНИЕ БАЗЫ И ДОБАВЛЕНИЕ ТЕСТОВЫХ ДАННЫХ
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Создаем базу данных...");

        // Создаем базу если не существует
        dbContext.Database.EnsureCreated();

        // Проверяем и добавляем тестовые данные
        SeedTestData(dbContext, logger);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"ОШИБКА: {ex.Message}");
    if (ex.InnerException != null)
        Console.WriteLine($"Внутренняя ошибка: {ex.InnerException.Message}");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Метод для добавления тестовых данных
static void SeedTestData(ApplicationDbContext context, ILogger logger)
{
    try
    {
        // Проверяем, есть ли уже категории
        if (!context.Categories.Any())
        {
            logger.LogInformation("Добавляем тестовые категории...");

            var categories = new List<Category>
            {
                new Category { Name = "Электроника" },
                new Category { Name = "Одежда" },
                new Category { Name = "Книги" },
                new Category { Name = "Дом и сад" }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();
            logger.LogInformation($"Добавлено {categories.Count} категорий.");
        }
        else
        {
            logger.LogInformation($"В базе уже есть {context.Categories.Count()} категорий.");
        }

        // Проверяем, есть ли уже продукты
        if (!context.Products.Any())
        {
            logger.LogInformation("Добавляем тестовые продукты...");

            // Получаем ID категорий
            var electronics = context.Categories.First(c => c.Name == "Электроника");
            var clothing = context.Categories.First(c => c.Name == "Одежда");
            var books = context.Categories.First(c => c.Name == "Книги");
            var home = context.Categories.First(c => c.Name == "Дом и сад");

            var products = new List<Product>
            {
                new Product { Name = "Ноутбук", Price = 99999, CategoryId = electronics.Id },
                new Product { Name = "Смартфон", Price = 49999, CategoryId = electronics.Id },
                new Product { Name = "Футболка", Price = 1999, CategoryId = clothing.Id },
                new Product { Name = "Джинсы", Price = 3999, CategoryId = clothing.Id },
                new Product { Name = "Программирование на C#", Price = 1499, CategoryId = books.Id },
                new Product { Name = "Диван", Price = 29999, CategoryId = home.Id },
                new Product { Name = "Стул", Price = 3999, CategoryId = home.Id },
                new Product { Name = "Наушники", Price = 7999, CategoryId = electronics.Id },
                new Product { Name = "Куртка", Price = 8999, CategoryId = clothing.Id },
                new Product { Name = "Книга по дизайну", Price = 2499, CategoryId = books.Id }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
            logger.LogInformation($"Добавлено {products.Count} продуктов.");
        }
        else
        {
            logger.LogInformation($"В базе уже есть {context.Products.Count()} продуктов.");
        }

        // Проверяем, есть ли уже изображения
        if (!context.ProductImages.Any() && context.Products.Any())
        {
            logger.LogInformation("Добавляем тестовые изображения...");

            var laptop = context.Products.First(p => p.Name == "Ноутбук");
            var phone = context.Products.First(p => p.Name == "Смартфон");
            var tshirt = context.Products.First(p => p.Name == "Футболка");
            var jeans = context.Products.First(p => p.Name == "Джинсы");
            var book = context.Products.First(p => p.Name == "Программирование на C#");

            var images = new List<ProductImage>
            {
                new ProductImage { ProductId = laptop.Id, ImageUrl = "/images/laptop.jpg" },
                new ProductImage { ProductId = phone.Id, ImageUrl = "/images/phone.jpg" },
                new ProductImage { ProductId = tshirt.Id, ImageUrl = "/images/tshirt.jpg" },
                new ProductImage { ProductId = jeans.Id, ImageUrl = "/images/jeans.jpg" },
                new ProductImage { ProductId = book.Id, ImageUrl = "/images/book.jpg" }
            };

            context.ProductImages.AddRange(images);
            context.SaveChanges();
            logger.LogInformation($"Добавлено {images.Count} изображений.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError($"Ошибка при добавлении тестовых данных: {ex.Message}");
    }
}