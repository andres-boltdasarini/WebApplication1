// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using BookingAgentApp.Models;
using BookingAgentApp.Data;
using BookingAgentApp.Services;
using NodaTime;

namespace BookingAgentApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CalendarService _calendarService;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
            _calendarService = new CalendarService();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAgent()
        {
            ViewBag.FreeAgents = _context.BookingAgents
                .Where(a => a.Status == "Свободен")
                .ToList();
            return View(new AgentRequest());
        }

        public IActionResult Agents()
        {
            var agents = _context.BookingAgents.ToList();
            return View(agents);
        }

        public IActionResult AgentDetails(int id)
        {
            var agent = _context.BookingAgents.FirstOrDefault(a => a.Id == id);
            
            if (agent == null)
            {
                return NotFound();
            }
            
            return View(agent);
        }

        // GET: Calendar для конкретного агента
        public IActionResult BookAgent(int id, int? year, int? month)
        {
            var agent = _context.BookingAgents.FirstOrDefault(a => a.Id == id);
            if (agent == null)
            {
                return NotFound();
            }

            var currentYear = year ?? DateTime.Now.Year;
            var currentMonth = month ?? DateTime.Now.Month;

            var calendar = _calendarService.GenerateCalendar(
                id, 
                currentYear, 
                currentMonth,
                agent.StartDate,
                agent.EndDate
            );

            ViewBag.Agent = agent;
            return View(calendar);
        }

        [HttpPost]
        public IActionResult BookAgent(int id, DateTime startDate, DateTime endDate)
        {
            var agent = _context.BookingAgents.Find(id);
            if (agent == null)
            {
                return NotFound();
            }

            // Конвертируем DateTime в Instant (UTC)
            var instantStartDate = Instant.FromDateTimeUtc(DateTime.SpecifyKind(startDate, DateTimeKind.Utc));
            var instantEndDate = Instant.FromDateTimeUtc(DateTime.SpecifyKind(endDate, DateTimeKind.Utc));

            // Проверяем, что даты не пересекаются с другими бронированиями
            var isAvailable = !_context.BookingAgents.Any(a => 
                a.Id != id && 
                a.Status == "Занят" &&
                a.StartDate.HasValue && a.EndDate.HasValue &&
                ((instantStartDate >= a.StartDate && instantStartDate <= a.EndDate) ||
                 (instantEndDate >= a.StartDate && instantEndDate <= a.EndDate) ||
                 (instantStartDate <= a.StartDate && instantEndDate >= a.EndDate)));

            if (!isAvailable)
            {
                TempData["ErrorMessage"] = "Выбранные даты пересекаются с другими бронированиями";
                return RedirectToAction(nameof(BookAgent), new { id });
            }

            agent.StartDate = instantStartDate;
            agent.EndDate = instantEndDate;
            agent.Status = "Занят";
            agent.BookedBy = User.Identity?.Name ?? "Пользователь";
            agent.BookingTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            
            _context.Update(agent);
            _context.SaveChanges();

            // Для отображения конвертируем Instant обратно в LocalDate
            var localStartDate = instantStartDate.InUtc().Date;
            var localEndDate = instantEndDate.InUtc().Date;
            
            TempData["SuccessMessage"] = $"Агент {agent.Name} забронирован с {localStartDate.Day:00}.{localStartDate.Month:00}.{localStartDate.Year} по {localEndDate.Day:00}.{localEndDate.Month:00}.{localEndDate.Year}";
            return RedirectToAction(nameof(AgentDetails), new { id });
        }

        [HttpPost]
        public IActionResult GetAgent(AgentRequest request)
        {
            if (ModelState.IsValid)
            {
                var agent = _context.BookingAgents.Find(request.AgentId);
                
                // Конвертируем DateTime в Instant (UTC)
                var instantStartDate = Instant.FromDateTimeUtc(DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc));
                var instantEndDate = Instant.FromDateTimeUtc(DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc));
                
                // Проверяем доступность дат
                var isDateAvailable = !_context.BookingAgents.Any(a => 
                    a.Id != request.AgentId && 
                    a.Status == "Занят" &&
                    a.StartDate.HasValue && a.EndDate.HasValue &&
                    ((instantStartDate >= a.StartDate && instantStartDate <= a.EndDate) ||
                     (instantEndDate >= a.StartDate && instantEndDate <= a.EndDate) ||
                     (instantStartDate <= a.StartDate && instantEndDate >= a.EndDate)));

                if (agent != null && agent.Status == "Свободен" && isDateAvailable)
                {
                    agent.Status = "Занят";
                    agent.BookedBy = request.UserName;
                    agent.BookingTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                    agent.StartDate = instantStartDate;
                    agent.EndDate = instantEndDate;
                    
                    _context.Update(agent);
                    _context.SaveChanges();
                    
                    TempData["SuccessMessage"] = $"Агент {agent.Name} успешно забронирован";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Агент или выбранные даты недоступны");
                }
            }
            
            ViewBag.FreeAgents = _context.BookingAgents
                .Where(a => a.Status == "Свободен")
                .ToList();
                
            return View(request);
        }

        [HttpPost]
        public IActionResult ReleaseAgent(int id)
        {
            var agent = _context.BookingAgents.Find(id);
            
            if (agent == null)
            {
                return NotFound();
            }

            if (agent.Status == "Занят")
            {
                agent.Status = "Свободен";
                agent.BookedBy = null;
                agent.BookingTime = null;
                agent.StartDate = null;
                agent.EndDate = null;
                
                _context.Update(agent);
                _context.SaveChanges();
                
                TempData["SuccessMessage"] = $"Агент {agent.Name} успешно освобожден";
            }
            
            return RedirectToAction(nameof(AgentDetails), new { id });
        }
    }
}