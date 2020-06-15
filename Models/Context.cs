using Microsoft.EntityFrameworkCore;

namespace LoginAndRegistration.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options) {}

        // Add tables here
        public DbSet<User> Users { get;set; }
    }
}