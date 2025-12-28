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
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageRepository _messageRepository;

        public UserController(
            IUserRepository userRepository,
            IOrderRepository orderRepository,
            IMessageRepository messageRepository)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _messageRepository = messageRepository;
        }

        public async Task<IActionResult> Profile(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid user id");

            // Проверка существования пользователя
            var userExists = await _userRepository.ExistsAsync(id);
            if (!userExists)
                return NotFound();

            try
            {
                // Параллельная загрузка данных
                var userTask = _userRepository.GetByIdAsync(id);
                var ordersTask = _orderRepository.GetByUserIdAsync(id);
                var messagesTask = _messageRepository.GetByToUserIdAsync(id);

                await Task.WhenAll(userTask, ordersTask, messagesTask);

                var user = await userTask;
                var orders = await ordersTask;
                var messages = await messagesTask;

                // Бизнес-логика
                if (orders?.Any(o => o.Total > 1000) == true)
                    user.IsVIP = true;

                var viewModel = new ProfileViewModel
                {
                    User = user,
                    Orders = orders,
                    UnreadMessages = messages?.Count(m => !m.IsRead) ?? 0
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                // Можно использовать ILogger для логирования ex
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}