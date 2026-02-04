using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Tasks.Any())
            {
                var tasks = new TaskItem[]
                {
                    new TaskItem
                    {
                        Title = "Пример задачи 1",
                        Description = "Это пример задачи",
                        CreatedDate = DateTime.Now.AddDays(-2),
                        IsCompleted = false
                    },
                    new TaskItem
                    {
                        Title = "Пример задачи 2",
                        Description = "Завершенная задача",
                        CreatedDate = DateTime.Now.AddDays(-1),
                        IsCompleted = true
                    },
                    new TaskItem
                    {
                        Title = "Пример задачи 3",
                        Description = "Еще одна активная задача",
                        CreatedDate = DateTime.Now,
                        IsCompleted = false
                    }
                };

                context.Tasks.AddRange(tasks);
                context.SaveChanges();
            }
        }
    }
}