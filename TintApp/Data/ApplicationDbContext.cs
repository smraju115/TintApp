using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using TintApp.Models;

namespace TintApp.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options) { }

        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<ServiceItem> ServiceItems { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.ServiceItem)
                .WithMany()
                .HasForeignKey(b => b.ServiceItemId)
                .OnDelete(DeleteBehavior.Restrict); // 👈 এখানে Restrict দাও
        }

    }

   
}
