using Mcce.SmartOffice.Core.Configs;
using Mcce.SmartOffice.UserImages.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.UserImages
{
    public class AppDbContext : DbContext
    {
        private readonly IAppConfig _appConfig;

        public DbSet<UserImage> UserImages { get; set; }

        public AppDbContext(DbContextOptions options, IAppConfig appConfig)
            : base(options)
        {
            _appConfig = appConfig;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_appConfig.DbConfig.DatabaseSchema);
        }
    }
}
