using Microsoft.AspNetCore.Mvc;
using BookingAgentApp.Models;
using BookingAgentApp.Data;

namespace BookingAgentApp.Controllers
{
    public class HomeController : Controller

    {

                private readonly ApplicationDbContext _context;
        private static int _nextId = 1;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetAgent()
        {
            return View();
        }

         [HttpPost]
        public IActionResult GetAgent(AgentRequest request)
        {
            if (ModelState.IsValid)
            {
                var agent = _context.BookingAgents.Find(request.AgentId);
                if (agent != null && agent.Status == "Свободен")
                {
                    agent.Status = "Занят";
                    agent.BookedBy = request.UserName;
                    agent.BookingTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                    
                    _context.Update(agent);
                    _context.SaveChanges();
                    
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(request);
        }
    }
}