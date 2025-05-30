﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReserveApp.Models;

namespace ReserveApp.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Resource> Resources { get; set; }
        public DbSet<UserResource> UserResources { get; set; }
        public DbSet<UserHistory> UserHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserResource>()
                .HasKey(ur => new { ur.UserId, ur.ResourceId });

            modelBuilder.Entity<UserResource>()
                .HasOne(ur => ur.Resource)
                .WithMany(r => r.UserResources)
                .HasForeignKey(ur => ur.ResourceId);
        }
    }
}