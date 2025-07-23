using Microsoft.EntityFrameworkCore;

namespace Travel.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TourProduct> TourProducts { get; set; }
    }
} 