// Program.cs
using Microsoft.EntityFrameworkCore;
using BookingAgentApp.Data;
using BookingAgentApp.Models;
using Npgsql;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL; // Добавьте этот using

var builder = WebApplication.CreateBuilder(args);

// Добавляем DbContext с PostgreSQL и поддержкой NodaTime
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        npgsqlOptionsAction =>
        {
            npgsqlOptionsAction.UseNodaTime(); // Включаем поддержку NodaTime
        });
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Создаем базу данных и таблицы если их нет
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Гарантированно создает базу данных и таблицы если их нет
    dbContext.Database.EnsureCreated();

    // Проверяем, есть ли данные в таблице
    if (!dbContext.BookingAgents.Any())
    {
        // Добавляем начальные данные
        dbContext.BookingAgents.AddRange(
            new BookingAgent
            {
                Name = "Агент 1 - Стенд s12",
                Status = "Свободен",
                ConnectionCommand = "git/testo/virt-qa-stand/xtesto s12 u3"
            },
            new BookingAgent
            {
                Name = "Агент 2 - Стенд s13",
                Status = "Свободен",
                ConnectionCommand = "git/testo/virt-qa-stand/xtesto s13 u3"
            },
            new BookingAgent
            {
                Name = "Агент 3 - Стенд s14",
                Status = "Свободен",
                ConnectionCommand = "git/testo/virt-qa-stand/xtesto s14 u3"
            }
        );

        dbContext.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();