using EventProcessor.Models;
using Microsoft.EntityFrameworkCore;

namespace EventProcessor.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // один-ко-многим 
            modelBuilder.Entity<Incident>()
                .HasMany(i => i.Events)
                .WithOne(e => e.Incident)
                .HasForeignKey(e => e.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // индексы
            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Time);

            modelBuilder.Entity<Incident>()
                .HasIndex(i => i.Time);
        }
    }
}
