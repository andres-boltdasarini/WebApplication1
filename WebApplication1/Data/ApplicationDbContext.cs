using Microsoft.EntityFrameworkCore;
using BookingAgentApp.Models;

namespace BookingAgentApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BookingAgent> BookingAgents { get; set; }
    }
}