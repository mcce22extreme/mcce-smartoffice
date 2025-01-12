﻿using Mcce.SmartOffice.Bookings.Entities;
using Mcce.SmartOffice.Core;
using Mcce.SmartOffice.Core.Accessors;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Bookings
{
    public class AppDbContext : AppDbContextBase
    {
        internal const string DATABASE_SCHEMA = "sobo";

        public DbSet<Booking> Bookings { get; set; }

        public AppDbContext(DbContextOptions options, IAuthContextAccessor contextAccessor)
            : base(options, contextAccessor)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DATABASE_SCHEMA);

            modelBuilder.Entity<Booking>()
                .HasIndex(x => x.BookingNumber)
                .IsUnique();
            ;
        }
    }
}
