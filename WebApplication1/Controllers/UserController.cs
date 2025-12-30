using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserProfileService _userProfileService;

        public UserController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        public async Task<IActionResult> Profile(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid user id");

            try
            {

                  var viewModel = await _userProfileService.GetUserProfileAsync(id);

                if (viewModel == null)
                    return NotFound($"Пользователь с ID {id} не найден");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Для отладки показываем детали ошибки
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    return Content($"Ошибка: {ex.Message}<br>StackTrace: {ex.StackTrace}");
                }
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}