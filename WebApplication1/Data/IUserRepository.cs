using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}