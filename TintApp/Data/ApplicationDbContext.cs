using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using System.Diagnostics.Metrics;
using TintApp.Models;

namespace TintApp.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options) { }

        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<ServiceItem> ServiceItems { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.ServiceItem)
                .WithMany()
                .HasForeignKey(b => b.ServiceItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Global filters
            modelBuilder.Entity<ServiceCategory>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ServiceItem>().HasQueryFilter(s => !s.IsDeleted);


        }

    }

   
}
