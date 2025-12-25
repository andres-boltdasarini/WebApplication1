using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // Главная страница с меню
        public IActionResult Index()
        {
            return View();
        }

        // 1. Клиенты с заказами
        public IActionResult CustomersWithOrders()
        {
            var customers = _context.Customers
                .Where(c => c.Orders.Any())
                .ToList();

            return View(customers);
        }

        // 2. Заказы за последние 30 дней
        public IActionResult RecentOrders()
        {
            // Измените DateTime.Now на DateTime.UtcNow
            var recentOrders = _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.OrderDate >= DateTime.UtcNow.AddDays(-30))
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(recentOrders);
        }

        // 3. Общая сумма заказов для клиента
        public IActionResult CustomerTotal(int id = 1)
        {
            decimal totalAmount = _context.Customers
                .Where(c => c.Id == id)
                .SelectMany(c => c.Orders)
                .Sum(o => o.TotalAmount);

            var customer = _context.Customers.Find(id);

            ViewBag.Customer = customer;
            ViewBag.TotalAmount = totalAmount;

            return View();
        }

        // 4. Клиенты с их последним заказом
        public IActionResult CustomersLastOrder()
        {
            var customersWithLastOrder = _context.Customers
                .Select(c => new
                {
                    Customer = c,
                    LastOrder = c.Orders
                        .OrderByDescending(o => o.OrderDate)
                        .FirstOrDefault()
                })
                .AsEnumerable() // Переключаем на выполнение в памяти
                .Select(x => new CustomerLastOrderDTO
                {
                    Id = x.Customer.Id,
                    Name = x.Customer.Name,
                    Email = x.Customer.Email,
                    LastOrderDate = x.LastOrder?.OrderDate,
                    LastOrderAmount = x.LastOrder?.TotalAmount ?? 0
                })
                .ToList();

            return View(customersWithLastOrder);
        }

        // 5. Клиенты со средней суммой заказа > 1000
        public IActionResult WealthyCustomers()
        {
            var wealthyCustomers = _context.Customers
                .Where(c => c.Orders.Any() && c.Orders.Average(o => o.TotalAmount) > 1000)
                .Select(c => new WealthyCustomerDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    AverageOrderAmount = c.Orders.Average(o => o.TotalAmount),
                    OrderCount = c.Orders.Count,
                    TotalSpent = c.Orders.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(c => c.AverageOrderAmount)
                .ToList();

            return View(wealthyCustomers);
        }
    }
}