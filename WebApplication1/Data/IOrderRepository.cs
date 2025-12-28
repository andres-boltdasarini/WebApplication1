using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
    }
}