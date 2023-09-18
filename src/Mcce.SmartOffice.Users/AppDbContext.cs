using Mcce.SmartOffice.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Users
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            modelBuilder.Entity<User>()
                .HasIndex(x => x.UserName)
                .IsUnique();
        }
    }
}
