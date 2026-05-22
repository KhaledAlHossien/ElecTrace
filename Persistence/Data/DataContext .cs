using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using System.Text;

namespace Persistence.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<SystemInfo> SystemInfos { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Role>()
            .HasIndex(r => r.Name)
            .IsUnique();

            builder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);


            builder.Entity<UserToken>()
                .HasOne(ut => ut.User)
                .WithMany()
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Department>()
                .HasIndex(d => d.MeterCode)
                .IsUnique();
            builder.Entity<Department>()
                .Property(d => d.Discount)
                .HasDefaultValue(0);

            builder.Entity<Role>().HasData(
                  new Role { Id = 1, Name = "Admin" },
                   new Role { Id = 2, Name = "Employee" }
            );



        }
    }
}
