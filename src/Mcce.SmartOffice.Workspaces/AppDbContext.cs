using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Workspaces.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Workspaces
{
    public class AppDbContext : AppDbContextBase
    {
        internal const string DATABASE_SCHEMA = "sows";

        public DbSet<Workspace> Workspaces { get; set; }

        public AppDbContext(DbContextOptions options, IHttpContextAccessor contextAccessor)
            : base(options, contextAccessor)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DATABASE_SCHEMA);

            modelBuilder.Entity<Workspace>()                
                .HasIndex(x => x.WorkspaceNumber)
                .IsUnique();
        }
    }
}
