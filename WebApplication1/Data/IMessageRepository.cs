using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetByToUserIdAsync(int userId);
    }
}