using BookingPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Resources> Resources { get; set; }
        public virtual  DbSet<Admin> Admins{ get; set; }
    }
}
