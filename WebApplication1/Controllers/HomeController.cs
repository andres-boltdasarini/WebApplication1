using Microsoft.AspNetCore.Mvc;
using PalindromeChecker.Models;

namespace PalindromeChecker.Controllers
{
    public class HomeController : Controller
    {
        private readonly PalindromeModel _model;

        public HomeController()
        {
            _model = new PalindromeModel();
        }

        public IActionResult Index()
        {
            var viewModel = new IndexViewModel
            {
                Examples = _model.GetPalindromeExamples()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Check(string inputWord, string selectedExample)
        {
            // Если выбран пример из списка, используем его
            string wordToCheck = !string.IsNullOrEmpty(selectedExample)
                ? selectedExample
                : inputWord ?? "";

            var result = _model.CheckPalindrome(wordToCheck);

            var viewModel = new IndexViewModel
            {
                InputWord = wordToCheck,
                Result = result,
                Examples = _model.GetPalindromeExamples(),
                SelectedExample = selectedExample
            };

            return View("Index", viewModel);
        }

        public IActionResult About()
        {
            ViewData["Title"] = "О программе";
            ViewData["Message"] = "Проверка палиндромов";

            return View();
        }

        public IActionResult HowItWorks()
        {
            ViewData["Title"] = "Как это работает";

            return View();
        }
    }
}