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
            if (userId <= 0)
                throw new ArgumentException("User ID must be positive", nameof(userId));

            try
            {
                // Создаем CancellationTokenSource для отмены всех задач при ошибке
                var cancellationTokenSource = new CancellationTokenSource();

                // Запускаем задачи с токеном отмены
                var userTask = _userRepository.GetByIdAsync(userId);
                var ordersTask = _orderRepository.GetByUserIdAsync(userId);
                var messagesTask = _messageRepository.GetByToUserIdAsync(userId);

                try
                {
                    await Task.WhenAll(userTask, ordersTask, messagesTask)
                        .WaitAsync(TimeSpan.FromSeconds(30), cancellationTokenSource.Token);
                }
                catch (TimeoutException)
                {
                    cancellationTokenSource.Cancel();
                    throw new TimeoutException("Timeout while loading user profile data");
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException("Profile loading was cancelled");
                }

                var user = await userTask;
                var orders = await ordersTask;
                var messages = await messagesTask;

                // Проверяем, что пользователь существует
                if (user == null)
                    return null;

                // Применяем бизнес-логику VIP статуса
                var userForViewModel = ApplyVipBusinessLogic(user, orders);

                return new ProfileViewModel
                {
                    User = userForViewModel,
                    Orders = orders ?? Enumerable.Empty<Order>(),
                    UnreadMessages = messages?.Count(m => !m.IsRead) ?? 0
                };
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                // Логируем исключение
                throw new ProfileServiceException($"Error loading profile for user {userId}", ex);
            }
        }

        // Кастомное исключение для сервиса
        public class ProfileServiceException : Exception
        {
            public ProfileServiceException(string message, Exception innerException = null)
                : base(message, innerException) { }
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