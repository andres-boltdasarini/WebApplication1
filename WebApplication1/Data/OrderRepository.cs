using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Order>(
                "SELECT * FROM orders WHERE userid = @UserId",
                new { UserId = userId });
        }
    }
}