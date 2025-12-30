using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting; // Добавляем using
using Microsoft.Extensions.Logging;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<UserController> _logger;
        private readonly IWebHostEnvironment _environment; // Добавляем для IsDevelopment

        public UserController(
            IUserProfileService userProfileService,
            ILogger<UserController> logger,
            IWebHostEnvironment environment) // Добавляем в конструктор
        {
            _userProfileService = userProfileService;
            _logger = logger;
            _environment = environment;
        }

        public async Task<IActionResult> Profile(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID requested: {UserId}", id);
                return BadRequest(new { Error = "Invalid user ID. ID must be positive." });
            }

            try
            {
                _logger.LogInformation("Processing profile request for user {UserId}", id);
                var viewModel = await _userProfileService.GetUserProfileAsync(id);

                if (viewModel == null)
                {
                    _logger.LogWarning("User not found: {UserId}", id);
                    return NotFound(new { Error = $"User with ID {id} not found" });
                }

                _logger.LogInformation("Profile view model created for user {UserId}", id);
                return View(viewModel);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for user profile");
                return BadRequest(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Profile service error for user {UserId}", id);

                // Используем _environment вместо Environment
                if (_environment.IsDevelopment())
                {
                    return StatusCode(500, new
                    {
                        Error = "Internal server error",
                        Details = ex.Message,
                        StackTrace = ex.StackTrace
                    });
                }

                return StatusCode(500, new { Error = "An error occurred while loading profile" });
            }
            catch (TimeoutException ex)
            {
                _logger.LogWarning(ex, "Timeout loading profile for user {UserId}", id);
                return StatusCode(504, new { Error = "Request timeout. Please try again." });
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "Operation cancelled for user {UserId}", id);
                return StatusCode(499, new { Error = "Request was cancelled" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error loading profile for user {UserId}", id);

                if (_environment.IsDevelopment())
                {
                    return StatusCode(500, new
                    {
                        Error = "Unexpected error",
                        Message = ex.Message,
                        Type = ex.GetType().Name
                    });
                }

                return StatusCode(500, new { Error = "An unexpected error occurred" });
            }
        }
    }
}