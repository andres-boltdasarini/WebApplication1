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
            // Инициализируем модель с датами по умолчанию (сегодня и завтра)
            var model = new AgentRequest
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            };

            return View(model);
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

        [HttpPost]
        public IActionResult GetAgent(AgentRequest request)
        {
            if (ModelState.IsValid)
            {
                // Проверка что дата окончания не раньше даты начала
                if (request.EndDate < request.StartDate)
                {
                    ModelState.AddModelError("EndDate", "Дата окончания не может быть раньше даты начала");
                    return View(request);
                }

                // Конвертируем DateTime в Instant (UTC)
                var instantStartDate = Instant.FromDateTimeUtc(
                    DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc));
                var instantEndDate = Instant.FromDateTimeUtc(
                    DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc));

                // Находим всех свободных агентов, соответствующих запросу
                var availableAgents = _context.BookingAgents
                    .Where(a => a.Status == "Свободен")
                    .AsEnumerable()  // Переключаемся на клиентскую оценку для MatchesRequest
                    .Where(a => a.MatchesRequest(request))
                    .ToList();

                if (!availableAgents.Any())
                {
                    ModelState.AddModelError("", "Нет доступных агентов, соответствующих выбранным параметрам");
                    return View(request);
                }

                // Выбираем первого подходящего агента
                var selectedAgent = availableAgents.First();

                // Обновляем данные агента
                selectedAgent.Status = "Занят";
                selectedAgent.BookedBy = request.UserMail;
                selectedAgent.BookingTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                selectedAgent.StartDate = instantStartDate;
                selectedAgent.EndDate = instantEndDate;

                // Сохраняем параметры запроса (хотя они и так уже есть в агенте)
                selectedAgent.Notif = request.Notif;
                selectedAgent.Copy = request.Copy;

                _context.Update(selectedAgent);
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Агент {selectedAgent.Name} успешно забронирован";
                return RedirectToAction(nameof(AgentDetails), new { id = selectedAgent.Id });
            }

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

        // Остальные методы (BookAgent, Calendar и т.д.) можно оставить без изменений
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
        public IActionResult BookAgent(int id, DateTime startDate, DateTime endDate, AgentRequest request)
        {
            // Существующая логика...
            var agent = _context.BookingAgents.Find(id);
            if (agent == null)
            {
                return NotFound();
            }

            var instantStartDate = Instant.FromDateTimeUtc(DateTime.SpecifyKind(startDate, DateTimeKind.Utc));
            var instantEndDate = Instant.FromDateTimeUtc(DateTime.SpecifyKind(endDate, DateTimeKind.Utc));

            // Проверяем доступность агента на эти даты
            bool isAvailable;

            if (agent.Status == "Свободен")
            {
                isAvailable = true;
            }
            else if (agent.StartDate.HasValue && agent.EndDate.HasValue)
            {
                isAvailable = !(
                    (instantStartDate >= agent.StartDate && instantStartDate <= agent.EndDate) ||
                    (instantEndDate >= agent.StartDate && instantEndDate <= agent.EndDate) ||
                    (instantStartDate <= agent.StartDate && instantEndDate >= agent.EndDate)
                );
            }
            else
            {
                isAvailable = false;
            }

            if (!isAvailable)
            {
                TempData["ErrorMessage"] = "Агент недоступен на выбранные даты";
                return RedirectToAction(nameof(BookAgent), new { id });
            }

            agent.StartDate = instantStartDate;
            agent.EndDate = instantEndDate;
            agent.Status = "Занят";
            agent.BookedBy = request?.UserMail ?? User.Identity?.Name ?? "Пользователь";
            agent.BookingTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

            if (request != null)
            {
                agent.Notif = request.Notif;
                agent.Copy = request.Copy;
            }

            _context.Update(agent);
            _context.SaveChanges();

            var localStartDate = instantStartDate.InUtc().Date;
            var localEndDate = instantEndDate.InUtc().Date;

            TempData["SuccessMessage"] = $"Агент {agent.Name} забронирован с {localStartDate.Day:00}.{localStartDate.Month:00}.{localStartDate.Year} по {localEndDate.Day:00}.{localEndDate.Month:00}.{localEndDate.Year}";
            return RedirectToAction(nameof(AgentDetails), new { id });
        }
    }
}