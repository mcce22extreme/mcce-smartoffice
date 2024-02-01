using Mcce.SmartOffice.Api.Accessors;
using Mcce.SmartOffice.Api.Entities;
using Mcce.SmartOffice.Api.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Api
{
    public class AppDbContext : DbContext
    {
        private readonly IAuthContextAccessor _contextAccessor;

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<UserImage> UserImages { get; set; }

        public DbSet<WorkspaceConfiguration> WorkspaceConfigurations { get; set; }

        public DbSet<WorkspaceDataEntry> WorkspaceDataEntries { get; set; }

        public DbSet<Workspace> Workspaces { get; set; }

        public AppDbContext(DbContextOptions options, IAuthContextAccessor contextAccessor)
            : base(options)
        {
            _contextAccessor = contextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .HasIndex(x => x.BookingNumber)
                .IsUnique();

            modelBuilder.Entity<Workspace>()
                .HasIndex(x => x.WorkspaceNumber)
                .IsUnique();

            modelBuilder.Entity<Workspace>()
                .HasMany(x => x.Configurations)
                .WithOne(x => x.Workspace)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Workspace>()
                .HasMany(x => x.DataEntries)
                .WithOne(x => x.Workspace)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkspaceConfiguration>()
                .HasIndex(x => new
                {
                    x.WorkspaceId,
                    x.UserName
                })
                .IsUnique();


        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default, AuditInfoUpdateMode auditInfoUpdateMode = AuditInfoUpdateMode.Update)
        {
            if (auditInfoUpdateMode == AuditInfoUpdateMode.Update)
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
