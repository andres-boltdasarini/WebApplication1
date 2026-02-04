using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Task
        public IActionResult Index(string filter = "all")
        {
            IQueryable<TaskItem> tasks = _context.Tasks;

            switch (filter.ToLower())
            {
                case "active":
                    tasks = tasks.Where(t => !t.IsCompleted);
                    break;
                case "completed":
                    tasks = tasks.Where(t => t.IsCompleted);
                    break;
            }

            ViewBag.Filter = filter;
            return View(tasks.OrderByDescending(t => t.CreatedDate).ToList());
        }

        // GET: Task/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaskItem task)
        {
            if (ModelState.IsValid)
            {
                task.CreatedDate = System.DateTime.Now;
                _context.Add(task);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // POST: Task/Toggle/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Toggle(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                _context.Update(task);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Task/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}