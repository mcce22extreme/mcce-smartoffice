﻿// <auto-generated />
using System;
using Mcce.SmartOffice.WorkspaceDataEntries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Mcce.SmartOffice.WorkspaceDataEntries.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230920194903_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("Mcce.SmartOffice.WorkspaceDataEntries.Entities.WorkspaceDataEntry", b =>
                {
                    b.Property<string>("EntryId")
                        .HasColumnType("TEXT");

                    b.Property<float>("Co2Level")
                        .HasColumnType("REAL");

                    b.Property<float>("Humidity")
                        .HasColumnType("REAL");

                    b.Property<float>("Temperature")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.Property<int>("Wei")
                        .HasColumnType("INTEGER");

                    b.Property<string>("WorkspaceNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("EntryId");

                    b.HasIndex("WorkspaceNumber");

                    b.ToTable("WorkspaceDataEntries");

                    b
                        .HasAnnotation("Cosmos:ContainerName", "WorkspaceDataEntries")
                        .HasAnnotation("Cosmos:PartitionKeyName", "EntryId");
                });
#pragma warning restore 612, 618
        }
    }
}