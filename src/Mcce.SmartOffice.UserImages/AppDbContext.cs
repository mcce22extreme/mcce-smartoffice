using Mcce.SmartOffice.UserImages.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.UserImages
{
    public class AppDbContext : DbContext
    {
        internal const string DATABASE_SCHEMA = "soui";

        public DbSet<UserImage> UserImages { get; set; }

        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DATABASE_SCHEMA);
        }
    }
}
