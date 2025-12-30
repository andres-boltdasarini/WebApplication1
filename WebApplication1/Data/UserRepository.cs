using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; // Добавляем using
using Npgsql;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IConfiguration configuration, ILogger<UserRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured");
            }
        }

        public async Task<User> GetByIdAsync(int id)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                return await connection.QueryFirstOrDefaultAsync<User>(
                    "SELECT * FROM users WHERE id = @Id",
                    new { Id = id },
                    commandTimeout: 30);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database error while getting user by ID {UserId}", id);
                throw new InvalidOperationException($"Error retrieving user with ID {id}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting user by ID {UserId}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id) // Добавляем недостающий метод
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var exists = await connection.ExecuteScalarAsync<bool>(
                    "SELECT EXISTS(SELECT 1 FROM users WHERE id = @Id)",
                    new { Id = id },
                    commandTimeout: 30);
                return exists;
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database error while checking if user exists {UserId}", id);
                throw new InvalidOperationException($"Error checking if user with ID {id} exists", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while checking if user exists {UserId}", id);
                throw;
            }
        }
    }
}