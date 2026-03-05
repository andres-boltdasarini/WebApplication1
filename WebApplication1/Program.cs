// Program.cs
using Microsoft.EntityFrameworkCore;
using BookingAgentApp.Data;
using BookingAgentApp.Models;
using Npgsql;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Добавляем DbContext с PostgreSQL и поддержкой NodaTime
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptionsAction =>
        {
            npgsqlOptionsAction.UseNodaTime();
        });
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Создаем базу данных и таблицы если их нет
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    dbContext.Database.EnsureCreated();

    // Проверяем, есть ли данные в таблице
    if (!dbContext.BookingAgents.Any())
    {
        // Добавляем начальные данные с жестко заданными параметрами
        dbContext.BookingAgents.AddRange(
            new BookingAgent
            {
                Name = "Агент 1 - Стенд s12 (X86_64, Рутокен ЭЦП)",
                Status = "Свободен",
                ConnectionCommand = "git/testo/virt-qa-stand/xtesto s12 u3",
                Arch = 1,           // X86_64
                TokenECP = 1,        // Рутокен ЭЦП
                TokenS = true,       // Есть Rutoken S
                Vscode = true,       // Есть VS Code
                Sublime = true,      // Есть Sublime
                Notif = true,        // Есть уведомления
                Copy = true          // Есть резервное копирование
            },
            new BookingAgent
            {
                Name = "Агент 2 - Стенд s13 (X86_64, JaCarta)",
                Status = "Свободен",
                ConnectionCommand = "git/testo/virt-qa-stand/xtesto s13 u3",
                Arch = 1,           // X86_64
                TokenECP = 2,        // JaCarta
                TokenS = false,      // Нет Rutoken S
                Vscode = false,      // Нет VS Code
                Sublime = false,     // Нет Sublime
                Notif = false,       // Нет уведомлений
                Copy = false         // Нет резервного копирования
            },
            new BookingAgent
            {
                Name = "Агент 3 - Стенд s14 (ARM, нет ECP токена)",
                Status = "Свободен",
                ConnectionCommand = "git/testo/virt-qa-stand/xtesto s14 u3",
                Arch = 2,           // ARM
                TokenECP = 0,        // Не нужен
                TokenS = true,       // Есть Rutoken S
                Vscode = true,       // Есть VS Code
                Sublime = true,      // Есть Sublime
                Notif = true,        // Есть уведомления
                Copy = true          // Есть резервное копирование
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