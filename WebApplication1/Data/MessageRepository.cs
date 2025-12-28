using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly string _connectionString;

        public MessageRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                             ?? configuration.GetConnectionString("Default");
        }

        public async Task<IEnumerable<Message>> GetByToUserIdAsync(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            // ИЗМЕНЕНО: touserid оставляем как есть (судя по структуре данных)
            return await connection.QueryAsync<Message>(
                "SELECT * FROM messages WHERE touserid = @UserId",
                new { UserId = userId });
        }
    }
}