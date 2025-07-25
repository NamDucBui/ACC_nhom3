using Microsoft.EntityFrameworkCore;
using TravelTourCrawler.Models;

namespace TravelTourCrawler.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Tour> Tours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Tour>()
            //    .HasIndex(t => t.Url)
            //    .IsUnique();
        }

    }
}
