using Microsoft.AspNetCore.Mvc;

namespace BookingAgentApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetAgent()
        {
            return View();
        }
    }
}