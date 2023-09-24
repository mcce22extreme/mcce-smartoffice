using Mcce.SmartOffice.UserImages.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.UserImages
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserImage> UserImages { get; set; }

        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserImage>()
                .HasPartitionKey(x => x.UserName)
                .HasNoDiscriminator()
                .ToContainer(nameof(UserImages));
        }
    }
}
