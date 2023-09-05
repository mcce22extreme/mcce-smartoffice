using Mcce.SmartOffice.Core.Entities;
using Mcce.SmartOffice.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Core
{
    public abstract class AppDbContextBase : DbContext
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public AppDbContextBase(DbContextOptions options, IHttpContextAccessor contextAccessor)
            : base(options)
        {
            _contextAccessor = contextAccessor;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddAuditInfo();

            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            AddAuditInfo();

            return base.SaveChanges();
        }
        
        private void AddAuditInfo()
        {
            var dateTimeNow = DateTime.UtcNow;
            var currentUser = _contextAccessor.GetUserInfo();

            var addedEntries = ChangeTracker.Entries<AuditableEntityBase>()
                .Where(x => x.State == EntityState.Added)
                .ToList();

            var modifiedEntries = ChangeTracker.Entries<AuditableEntityBase>()
                .Where(x => x.State == EntityState.Modified)
                .ToList();

            foreach (var entry in addedEntries)
            {
                entry.Entity.CreatedUtc = entry.Entity.ModifiedUtc = dateTimeNow;
                entry.Entity.Creator = entry.Entity.Modifier = currentUser.UserName;
            }

            foreach (var entry in modifiedEntries)
            {
                entry.Entity.ModifiedUtc = dateTimeNow;
                entry.Entity.Modifier = currentUser.UserName;
            }
        }
    }
}
