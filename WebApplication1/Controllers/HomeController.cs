using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PalindromeChecker.Models;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly InputProcessor _inputProcessor;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _inputProcessor = new InputProcessor(); // Создаем экземпляр процессора
        }

        public IActionResult Index()
        {
            // Создаем пустую модель для представления
            var viewModel = new IndexViewModel
            {
                InputNumber = ""
            };
            return View(viewModel);
        }

        [HttpPost] // Обработка POST-запроса
        public IActionResult Process(string inputNumber)
        {
            var viewModel = new IndexViewModel
            {
                InputNumber = inputNumber
            };

            if (!string.IsNullOrEmpty(inputNumber))
            {
                viewModel.Result = _inputProcessor.ProcessInput(inputNumber);
            }

            return View("Index", viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}