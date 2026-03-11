using Microsoft.EntityFrameworkCore;
using SoftwareLicenses.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareLicenses.Data
{
    public class AppDbContext : DbContext
    {
        private const string _connectionString =
            @"Data Source=(LocalDB)\MSSQLLocalDB;
          Initial Catalog=Lozovaya_SoftwareLicenses;
          Integrated Security=True;
          TrustServerCertificate=True;
          MultipleActiveResultSets=True";

        public DbSet<Software> Softwares => Set<Software>();
        public DbSet<License> Licenses => Set<License>();
        public DbSet<Device> Devices => Set<Device>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Installation> Installations => Set<Installation>();
        public DbSet<Account> Accounts => Set<Account>();


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
    .HasOne(a => a.Employee)
    .WithMany()
    .HasForeignKey(a => a.EmployeeId)
    .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Device>()
                .HasIndex(d => d.InventoryNumber)
                .IsUnique();

            modelBuilder.Entity<Installation>()
                .HasIndex(i => new { i.DeviceId, i.SoftwareId })
                .IsUnique();

            modelBuilder.Entity<License>()
                .HasOne(l => l.Software)
                .WithMany(s => s.Licenses)
                .HasForeignKey(l => l.SoftwareId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Installation>()
                .HasOne(i => i.Software)
                .WithMany(s => s.Installations)
                .HasForeignKey(i => i.SoftwareId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Installation>()
                .HasOne(i => i.Device)
                .WithMany(d => d.Installations)
                .HasForeignKey(i => i.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Installation>()
                .HasOne(i => i.License)
                .WithMany(l => l.Installations)
                .HasForeignKey(i => i.LicenseId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Installation>()
                .HasOne(i => i.InstalledByEmployee)
                .WithMany()
                .HasForeignKey(i => i.InstalledByEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
