using LoginApi.DTOs;
using LoginApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoginApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<Trajet> Trajets { get; set; }
        public DbSet<Vehicule> Vehicules { get; set; }
        public DbSet<Rapport> Rapports { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define composite key for UserRole
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // If you have navigation properties set up for many-to-many relationships
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        }
    }
}
