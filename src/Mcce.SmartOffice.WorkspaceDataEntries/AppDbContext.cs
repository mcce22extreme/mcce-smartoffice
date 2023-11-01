using Mcce.SmartOffice.Core.Configs;
using Mcce.SmartOffice.WorkspaceDataEntries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.WorkspaceDataEntries
{
    public class AppDbContext : DbContext
    {
        private readonly IAppConfig _appConfig;

        public DbSet<WorkspaceDataEntry> WorkspaceDataEntries { get; set; }

        public AppDbContext(DbContextOptions options, IAppConfig appConfig)
            : base(options)
        {
            _appConfig = appConfig;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_appConfig.DbConfig.DatabaseSchema);

            modelBuilder.Entity<WorkspaceDataEntry>()
                .HasIndex(x => x.WorkspaceNumber);
        }
    }
}
