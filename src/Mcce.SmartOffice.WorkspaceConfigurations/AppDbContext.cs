using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Configs;
using Mcce.SmartOffice.WorkspaceConfigurations.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.WorkspaceConfigurations
{
    public class AppDbContext : AppDbContextBase
    {
        private readonly IAppConfig _appConfig;

        public DbSet<WorkspaceConfiguration> WorkspaceConfigurations { get; set; }

        public AppDbContext(DbContextOptions options, IHttpContextAccessor contextAccessor, IAppConfig appConfig)
            : base(options, contextAccessor)
        {
            _appConfig = appConfig;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_appConfig.DbConfig.DatabaseSchema);

            modelBuilder.Entity<WorkspaceConfiguration>()
                .HasIndex(x => new
                {
                    x.WorkspaceNumber,
                    x.UserName
                })
                .IsUnique();
        }
    }
}
