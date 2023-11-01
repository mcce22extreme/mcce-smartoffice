using Mcce.SmartOffice.Bookings.Entities;
using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Configs;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Bookings
{
    public class AppDbContext : AppDbContextBase
    {
        private readonly IAppConfig _appConfig;

        public DbSet<Booking> Bookings { get; set; }

        public AppDbContext(DbContextOptions options, IHttpContextAccessor contextAccessor, IAppConfig appConfig)
            : base(options, contextAccessor)
        {
            _appConfig = appConfig;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_appConfig.DbConfig.DatabaseSchema);

            modelBuilder.Entity<Booking>()
                .HasIndex(x => x.BookingNumber)
                .IsUnique();
            ;
        }
    }
}
