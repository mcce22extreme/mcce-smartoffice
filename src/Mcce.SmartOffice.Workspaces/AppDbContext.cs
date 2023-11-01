using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Configs;
using Mcce.SmartOffice.Workspaces.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Workspaces
{
    public class AppDbContext : AppDbContextBase
    {
        private readonly IAppConfig _appConfig;

        public DbSet<Workspace> Workspaces { get; set; }

        public AppDbContext(DbContextOptions options, IHttpContextAccessor contextAccessor, IAppConfig appConfig)
            : base(options, contextAccessor)
        {
            _appConfig = appConfig;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_appConfig.DbConfig.DatabaseSchema);

            modelBuilder.Entity<Workspace>()                
                .HasIndex(x => x.WorkspaceNumber)
                .IsUnique();
        }
    }
}
