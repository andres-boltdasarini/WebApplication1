// Program.cs
using Microsoft.EntityFrameworkCore;
using BookingAgentApp.Data;
using BookingAgentApp.Models;
using BookingAgentApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Добавляем аутентификацию
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// Добавляем DbContext с PostgreSQL и поддержкой NodaTime
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptionsAction =>
        {
            npgsqlOptionsAction.UseNodaTime();
        });
});

// Добавляем сервисы
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CalendarService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Важно: порядок middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Создаем базу данных и таблицы если их нет
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var authService = scope.ServiceProvider.GetRequiredService<AuthService>();

    // Убеждаемся что база данных создана
    dbContext.Database.EnsureCreated();

    // Проверяем, есть ли данные в таблице агентов
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

    // Создаем тестовых пользователей если нет пользователей
    if (!dbContext.Users.Any())
    {
        // Используем асинхронный метод синхронно для простоты
        Task.Run(async () =>
        {
            await authService.RegisterUser("user", "User123!", "user@example.com", "User");
            await authService.RegisterUser("admin", "Admin123!", "admin@example.com", "Admin");
        }).GetAwaiter().GetResult();
    }
}

app.Run();