using Mcce.SmartOffice.Core.Accessors;
using Mcce.SmartOffice.Core.Entities;
using Mcce.SmartOffice.Core.Enums;
using Mcce.SmartOffice.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Core
{
    public abstract class AppDbContextBase : DbContext
    {
        private readonly IAuthContextAccessor _contextAccessor;

        public AppDbContextBase(DbContextOptions options, IAuthContextAccessor contextAccessor)
            : base(options)
        {
            _contextAccessor = contextAccessor;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default, AuditInfoUpdateMode auditInfoUpdateMode = AuditInfoUpdateMode.Update)
        {
            if(auditInfoUpdateMode == AuditInfoUpdateMode.Update)
            {
                AddAuditInfo();
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return SaveChangesAsync(cancellationToken, AuditInfoUpdateMode.Update);
        }

        public int SaveChanges(AuditInfoUpdateMode auditInfoUpdateMode = AuditInfoUpdateMode.Update)
        {
            if (auditInfoUpdateMode == AuditInfoUpdateMode.Update)
            {
                AddAuditInfo();
            }

            return base.SaveChanges();
        }

        public override int SaveChanges()
        {
            return SaveChanges(AuditInfoUpdateMode.Update);
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
