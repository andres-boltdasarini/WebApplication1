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

                // Проверяем, что пользователь существует
                if (user == null)
                    return NotFound($"Пользователь с ID {id} не найден");

                // Создаем КОПИЮ пользователя для применения бизнес-логики
                var userForViewModel = new User
                {
                    Id = user.Id,
                    Name = user.Name,
                    IsVIP = user.IsVIP // Берем текущее значение
                };

                // Применяем бизнес-логику VIP статуса
                // Проверяем не null и не пустую коллекцию
                if (orders != null && orders.Any())
                {
                    // Если есть хотя бы один заказ > 1000, устанавливаем VIP
                    userForViewModel.IsVIP = orders.Any(o => o.Total > 1000);
                }

                var viewModel = new ProfileViewModel
                {
                    User = userForViewModel,
                    Orders = orders ?? Enumerable.Empty<Order>(),
                    UnreadMessages = messages?.Count(m => !m.IsRead) ?? 0
                };

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