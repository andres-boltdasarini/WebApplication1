using System;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); // Изменено здесь

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    if (!context.Customers.Any())
    {
        // Инициализация данных остается без изменений
        var customers = new[]
        {
            new Customer { Name = "Иван Иванов", Email = "ivan@test.com" },
            new Customer { Name = "Петр Петров", Email = "petr@test.com" },
            new Customer { Name = "Мария Сидорова", Email = "maria@test.com" }
        };

        context.Customers.AddRange(customers);
        context.SaveChanges();

        var orders = new[]
        {
            new Order {
                CustomerId = 1,
                OrderDate = DateTime.UtcNow.AddDays(-10),
                TotalAmount = 1500.50m
            },
            new Order {
                CustomerId = 1,
                OrderDate = DateTime.UtcNow.AddDays(-5),
                TotalAmount = 2500.75m
            },
            new Order {
                CustomerId = 2,
                OrderDate = DateTime.UtcNow.AddDays(-15),
                TotalAmount = 800.00m
            }
        };

        context.Orders.AddRange(orders);
        context.SaveChanges();
    }
}

// Настройка конвейера
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customer}/{action=Index}/{id?}"); // Убедитесь, что контроллер существует

app.Run();