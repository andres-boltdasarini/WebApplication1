// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using BookingAgentApp.Models;
using BookingAgentApp.Services;
using System.Diagnostics;
using WebApplication1.Models;

namespace BookingAgentApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAgentService _agentService;

        public HomeController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                Agents = _agentService.GetAllAgents(),
                Instructions = GetInstructions()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult BookAgent(AgentRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = _agentService.BookAgent(request.AgentId, request.UserName);
                if (result)
                {
                    TempData["SuccessMessage"] = "Агент успешно забронирован!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Не удалось забронировать агента";
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ReleaseAgent(int id)
        {
            var result = _agentService.ReleaseAgent(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Агент освобожден";
            }
            return RedirectToAction("Index");
        }

        private List<Instruction> GetInstructions()
        {
            return new List<Instruction>
            {
                new Instruction
                {
                    Title = "Подключение к агенту",
                    Description = "После бронирования агента подключиться к нему можно с помощью xtesto",
                    Steps = new List<string>
                    {
                        "Пример команды подключения:",
                        "git/testo/virt-qa-stand/xtesto s12 u3",
                        "Где s12 - номер стенда, u3 - номер юзера"
                    }
                },
                new Instruction
                {
                    Title = "Настройка окружения",
                    Description = "Выполните следующие команды для настройки рабочего окружения",
                    Steps = new List<string>
                    {
                        "Команды настройки окружения:"
                    },
                    Commands = new List<string>
                    {
                        "cd && cd git",
                        "git clone ssh://git@git.astralinux.ru:7999/qa/testo.git",
                        "git clone ssh://git@git.astralinux.ru:7999/qa/skts-test.git",
                        "cp /home/$USER/CONFIG.testo /home/$USER/git/testo/virt-qa-stand/CONFIG.testo",
                        "echo -e \"macro extra_list(){\\n\\n}\" > /home/$USER/git/testo/lib/extra_list.testo",
                        "touch /home/$USER/git/testo/lib/pkgs/sources.list.current"
                    }
                }
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class HomeViewModel
    {
        public List<BookingAgent> Agents { get; set; }
        public List<Instruction> Instructions { get; set; }
        public AgentRequest? Request { get; set; }
    }

    public class Instruction
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Steps { get; set; }
        public List<string> Commands { get; set; }
    }
}