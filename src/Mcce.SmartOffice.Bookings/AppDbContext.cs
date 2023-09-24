﻿using Mcce.SmartOffice.Bookings.Entities;
using Mcce.SmartOffice.Core;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Bookings
{
    public class AppDbContext : AppDbContextBase
    {
        public DbSet<Booking> Bookings { get; set; }

        public AppDbContext(DbContextOptions options, IHttpContextAccessor contextAccessor)
            : base(options, contextAccessor)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .HasPartitionKey(x => x.WorkspaceNumber)
                .HasNoDiscriminator()
                .ToContainer(nameof(Bookings))
                .HasIndex(x => x.BookingNumber)
                .IsUnique();
            ;
        }
    }
}
