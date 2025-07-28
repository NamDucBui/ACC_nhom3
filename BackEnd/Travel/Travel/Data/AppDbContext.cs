using Microsoft.EntityFrameworkCore;
using Travel.Models;

namespace Travel.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<Tour> Tours { get; set; }  // DbSet cho Tour, tương ứng với bảng trong CSDL

    }
}
