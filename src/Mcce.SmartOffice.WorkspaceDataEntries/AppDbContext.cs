using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.WorkspaceDataEntries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.WorkspaceDataEntries
{
    public class AppDbContext : AppDbContextBase
    {
        public DbSet<WorkspaceDataEntry> Entries { get; set; }

        public AppDbContext(DbContextOptions options, IHttpContextAccessor contextAccessor)
            : base(options, contextAccessor)
        {
        }
    }
}
