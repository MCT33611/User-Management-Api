using Microsoft.EntityFrameworkCore;
using User_Management_Api.Model;

namespace User_Management_Api.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
