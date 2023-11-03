using Mcce.SmartOffice.WorkspaceDataEntries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.WorkspaceDataEntries
{
    public class AppDbContext : DbContext
    {
        internal const string DATABASE_SCHEMA = "sowd";

        public DbSet<WorkspaceDataEntry> WorkspaceDataEntries { get; set; }

        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DATABASE_SCHEMA);

            modelBuilder.Entity<WorkspaceDataEntry>()
                .HasIndex(x => x.WorkspaceNumber);
        }
    }
}
