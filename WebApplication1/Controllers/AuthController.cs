using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookingAgentApp.Models;
using BookingAgentApp.Services;

namespace BookingAgentApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                if (await _authService.ValidateUser(model.Username, model.Password))
                {
                    await _authService.SignIn(model.Username, model.RememberMe);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Неверное имя пользователя или пароль");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

       
        [HttpGet]
        public async Task<IActionResult> CreateTestAdmin()
        {
            var result = await _authService.RegisterUser("admin", "Admin123!", "admin@example.com", "Admin");
            if (result)
            {
                TempData["SuccessMessage"] = "Тестовый администратор создан. Логин: admin, Пароль: Admin123!";
            }
            else
            {
                TempData["ErrorMessage"] = "Администратор уже существует";
            }
            return RedirectToAction("Login");
        }
    }
}