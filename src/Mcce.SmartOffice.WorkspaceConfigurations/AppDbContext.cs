using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.WorkspaceConfigurations.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.WorkspaceConfigurations
{
    public class AppDbContext : AppDbContextBase
    {
        public DbSet<WorkspaceConfiguration> WorkspaceConfigurations { get; set; }

        public AppDbContext(DbContextOptions options, IHttpContextAccessor contextAccessor)
            : base(options, contextAccessor)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkspaceConfiguration>()
                .HasPartitionKey(x => x.WorkspaceNumber)
                .HasNoDiscriminator()
                .ToContainer(nameof(WorkspaceConfigurations))
                .HasIndex(x => new
                {
                    x.WorkspaceNumber,
                    x.UserName
                })
                .IsUnique();
        }
    }
}
