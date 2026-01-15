using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PalindromeChecker.Models; // Добавьте это
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PalindromeModel _palindromeModel;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _palindromeModel = new PalindromeModel(); // Создаем экземпляр
        }

        public IActionResult Index()
        {
            // Создаем модель для представления
            var viewModel = new IndexViewModel
            {
                InputWord = ""
            };
            return View(viewModel); // Передаем модель в представление
        }

        [HttpPost] // Обработка POST-запроса
        public IActionResult Check(string inputWord)
        {
            var viewModel = new IndexViewModel
            {
                InputWord = inputWord
            };

            if (!string.IsNullOrEmpty(inputWord))
            {
                viewModel.Result = _palindromeModel.CheckPalindrome(inputWord);
            }

            return View("Index", viewModel); // Возвращаемся на Index с результатами
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