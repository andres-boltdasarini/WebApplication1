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
    public class MessageRepository : IMessageRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(IConfiguration configuration, ILogger<MessageRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                             ?? configuration.GetConnectionString("Default");
            _logger = logger;

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured");
            }
        }

        public async Task<IEnumerable<Message>> GetByToUserIdAsync(int userId)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                return await connection.QueryAsync<Message>(
                    "SELECT * FROM messages WHERE touserid = @UserId",
                    new { UserId = userId },
                    commandTimeout: 30);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database error while getting messages for user {UserId}", userId);
                throw new InvalidOperationException($"Error retrieving messages for user {userId}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting messages for user {UserId}", userId);
                throw;
            }
        }
    }
}