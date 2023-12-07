using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Accessors;
using Mcce.SmartOffice.WorkspaceConfigurations.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.WorkspaceConfigurations
{
    public class AppDbContext : AppDbContextBase
    {
        internal const string DATABASE_SCHEMA = "sowc";

        public DbSet<WorkspaceConfiguration> WorkspaceConfigurations { get; set; }

        public AppDbContext(DbContextOptions options, IAuthContextAccessor contextAccessor)
            : base(options, contextAccessor)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DATABASE_SCHEMA);

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
