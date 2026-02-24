using Microsoft.EntityFrameworkCore;
using SupporTicketManagement.Models;

namespace SupporTicketManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<TicketStatusLog> TicketStatusLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Role>(e =>
            {
                e.HasIndex(r => r.Name).IsUnique();
            });
            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique();
                e.HasOne(u => u.Role).WithMany(r => r.Users)
                 .HasForeignKey(u => u.RoleId).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Ticket>(e =>
            {
                e.HasOne(t => t.Creator).WithMany()
                 .HasForeignKey(t => t.CreatedBy).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(t => t.Assignee).WithMany()
                 .HasForeignKey(t => t.AssignedTo).OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<TicketComment>(e =>
            {
                e.HasOne(c => c.Ticket).WithMany(t => t.Comments)
                 .HasForeignKey(c => c.TicketId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(c => c.User).WithMany()
                 .HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<TicketStatusLog>(e =>
            {
                e.HasOne(l => l.Ticket).WithMany(t => t.StatusLogs)
                 .HasForeignKey(l => l.TicketId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(l => l.ChangedByUser).WithMany()
                 .HasForeignKey(l => l.ChangedBy).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "MANAGER" },
                new Role { Id = 2, Name = "SUPPORT" },
                new Role { Id = 3, Name = "USER" }
            );
            var fixedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Manager User",
                    Email = "manager@test.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Password@123"),
                    RoleId = 1,
                    CreatedAt = fixedDate
                },
                new User
                {
                    Id = 2,
                    Name = "Support User",
                    Email = "support@test.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Password@123"),
                    RoleId = 2,
                    CreatedAt = fixedDate
                },
                new User
                {
                    Id = 3,
                    Name = "Normal User",
                    Email = "user@test.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Password@123"),
                    RoleId = 3,
                    CreatedAt = fixedDate
                }
            );
        }
    }
}
