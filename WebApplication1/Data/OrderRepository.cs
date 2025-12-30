using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(IConfiguration configuration, ILogger<OrderRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                             ?? configuration.GetConnectionString("Default");
            _logger = logger;

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured");
            }
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                return await connection.QueryAsync<Order>(
                    "SELECT * FROM orders WHERE user_id = @UserId",
                    new { UserId = userId },
                    commandTimeout: 30);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database error while getting orders for user {UserId}", userId);
                throw new InvalidOperationException($"Error retrieving orders for user {userId}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting orders for user {UserId}", userId);
                throw;
            }
        }
    }
}