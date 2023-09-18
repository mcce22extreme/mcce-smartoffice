using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.WorkspaceDataEntries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.WorkspaceDataEntries
{
    public class AppDbContext : AppDbContextBase
    {
        public DbSet<WorkspaceDataEntry> WorkspaceDataEntries { get; set; }

        public AppDbContext(DbContextOptions options, IHttpContextAccessor contextAccessor)
            : base(options, contextAccessor)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkspaceDataEntry>()
                .HasPartitionKey(x => x.EntryId)
                .HasNoDiscriminator()
                .ToContainer(nameof(WorkspaceDataEntries));

            modelBuilder.Entity<WorkspaceDataEntry>()
                .HasIndex(x => x.WorkspaceNumber);
        }
    }
}
