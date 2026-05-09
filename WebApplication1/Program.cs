
using Microsoft.EntityFrameworkCore;
using BookingAgentApp.Data;
using BookingAgentApp.Models;
using BookingAgentApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);


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


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptionsAction =>
        {
            npgsqlOptionsAction.UseNodaTime();
        });
});


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CalendarService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var authService = scope.ServiceProvider.GetRequiredService<AuthService>();


    dbContext.Database.EnsureCreated();

    
    if (!dbContext.BookingAgents.Any())
    {
       
        dbContext.BookingAgents.AddRange(
            new BookingAgent
            {
                Name = "Агент 1 - Стенд a2 (X86_64, Рутокен ЭЦП)",
                Status = "Свободен",
                ConnectionCommand = "git/testo/virt-stand/xteso a2 u3",
                Arch = 1,           
                TokenECP = 1,        
                TokenS = true,       
                Vscode = true,      
                Sublime = true,      
                Notif = true,       
                Copy = true          
            },
            new BookingAgent
            {
                Name = "Агент 2 - Стенд a3 (X86_64, JaCarta)",
                Status = "Свободен",
                ConnectionCommand = "git/testo/virt-stand/xteso a3 u3",
                Arch = 1,           
                TokenECP = 2,        
                TokenS = false,      
                Vscode = false,      
                Sublime = false,     
                Notif = false,      
                Copy = false         
            },
            new BookingAgent
            {
                Name = "Агент 3 - Стенд a4 (ARM, нет ECP токена)",
                Status = "Свободен",
                ConnectionCommand = "git/testo/virt-stand/xteso a4 u3",
                Arch = 2,        
                TokenECP = 0,       
                TokenS = true,       
                Vscode = true,       
                Sublime = true,     
                Notif = true,        
                Copy = true          
            }
        );

        dbContext.SaveChanges();
    }


    if (!dbContext.Users.Any())
    {
      
        Task.Run(async () =>
        {
            await authService.RegisterUser("user", "123", "user@example.com", "User");
            await authService.RegisterUser("admin", "123", "admin@example.com", "Admin");
        }).GetAwaiter().GetResult();
    }
}

app.Run();