using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class AuthController : Controller
    {
        private static List<User> _users = new List<User>
        {
            new User { Id = 1, Username = "admin", Password = "admin123", Role = "Admin" },
            new User { Id = 2, Username = "user", Password = "user123", Role = "User" }
        };

        private static int _nextUserId = 3;
        private readonly JwtService _jwtService;
        private static readonly List<string> _roles = new List<string> { "User", "Admin" };

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginRequest loginRequest)
        {
            var user = _users.FirstOrDefault(u => u.Username == loginRequest.Username && u.Password == loginRequest.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Неверное имя пользователя или пароль");
                return View(loginRequest);
            }

            var token = _jwtService.GenerateToken(user);

            // Сохраняем токен в cookie
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddMinutes(60)
            });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Roles"] = new SelectList(_roles);
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (_users.Any(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Имя пользователя уже занято");
                ViewData["Roles"] = new SelectList(_roles);
                return View(user);
            }

            user.Id = _nextUserId++;
            _users.Add(user);

            // Автоматический вход после регистрации
            var token = _jwtService.GenerateToken(user);
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddMinutes(60)
            });

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }
    }
}