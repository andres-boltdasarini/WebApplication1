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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Явно указываем имя таблицы
            modelBuilder.Entity<BookingAgent>(entity =>
            {
                entity.ToTable("BookingAgents");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Status).HasDefaultValue("Свободен");
            });
        }
    }
}