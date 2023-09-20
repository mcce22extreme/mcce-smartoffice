﻿// <auto-generated />
using System;
using Mcce.SmartOffice.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Mcce.SmartOffice.Bookings.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230920194820_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("Mcce.SmartOffice.Bookings.Entities.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Activated")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ActivationCode")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Creator")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .HasColumnType("TEXT");

                    b.Property<bool>("InvitationSent")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Modifier")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT");

                    b.Property<string>("WorkspaceNumber")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Bookings");

                    b
                        .HasAnnotation("Cosmos:ContainerName", "Bookings")
                        .HasAnnotation("Cosmos:PartitionKeyName", "WorkspaceNumber");
                });
#pragma warning restore 612, 618
        }
    }
}
