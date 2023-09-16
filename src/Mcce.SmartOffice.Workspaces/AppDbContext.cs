using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Workspaces.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Workspaces
{
    public class AppDbContext : AppDbContextBase
    {
        public DbSet<Workspace> Workspaces { get; set; }

        public AppDbContext(DbContextOptions options, IHttpContextAccessor contextAccessor)
            : base(options, contextAccessor)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Workspace>()
                .HasPartitionKey(x => x.WorkspaceNumber)
                .HasNoDiscriminator()
                .ToContainer(nameof(Workspaces))
                .HasIndex(x => x.WorkspaceNumber)
                .IsUnique();
        }
    }
}
