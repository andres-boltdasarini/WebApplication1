using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Data
{

    public interface IUserProfileService
    {
        Task<ProfileViewModel> GetUserProfileAsync(int userId);
    }

    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageRepository _messageRepository;

        public UserProfileService(
            IUserRepository userRepository,
            IOrderRepository orderRepository,
            IMessageRepository messageRepository)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _messageRepository = messageRepository;
        }

        public async Task<ProfileViewModel> GetUserProfileAsync(int userId)
        {
            // Параллельная загрузка данных
            var userTask = _userRepository.GetByIdAsync(userId);
            var ordersTask = _orderRepository.GetByUserIdAsync(userId);
            var messagesTask = _messageRepository.GetByToUserIdAsync(userId);

            await Task.WhenAll(userTask, ordersTask, messagesTask);

            var user = await userTask;
            var orders = await ordersTask;
            var messages = await messagesTask;

            // Проверяем, что пользователь существует
            if (user == null)
                return null;

            // Применяем бизнес-логику VIP статуса
            var userForViewModel = ApplyVipBusinessLogic(user, orders);

            var viewModel = new ProfileViewModel
            {
                User = userForViewModel,
                Orders = orders ?? Enumerable.Empty<Order>(),
                UnreadMessages = messages?.Count(m => !m.IsRead) ?? 0
            };

            return viewModel;
        }

        private User ApplyVipBusinessLogic(User user, IEnumerable<Order> orders)
        {
            // Создаем КОПИЮ пользователя для применения бизнес-логики
            var userForViewModel = new User
            {
                Id = user.Id,
                Name = user.Name,
                IsVIP = user.IsVIP // Берем текущее значение
            };

            // Бизнес-логика VIP статуса
            // Если есть хотя бы один заказ > 1000, устанавливаем VIP
            if (orders != null && orders.Any())
            {
                userForViewModel.IsVIP = orders.Any(o => o.Total > 1000);
            }

            return userForViewModel;
        }
    }
}