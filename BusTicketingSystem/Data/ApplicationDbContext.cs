using BusTicketingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BusTicketingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasData(
                    new Role { RoleId = 1, Name = "Admin" },
                    new Role { RoleId = 2, Name = "Customer" }
                );

            modelBuilder.Entity<Bus>()
                .HasIndex(b => b.BusNumber)
                .IsUnique();

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FullName = "System Admin",
                    Email = "admin@system.com",
                    //PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    PasswordHash = "$2a$12$q3IlCTLc4..NdjaxvVHL.OVvMulc/lMJ306tHOJ0gXufE3GGdho76", // ← static
                    PhoneNumber = "9999999999",
                    RoleId = 1, // Admin
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 01, 01),
                    IsDeleted = false
                    }
                );

            modelBuilder.Entity<AuditLog>().HasKey(a => a.AuditId);

            modelBuilder.Entity<Models.Route>(entity =>
            {
                entity.HasKey(r => r.RouteId);

                entity.Property(r => r.Source)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(r => r.Destination)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(r => r.IsActive)
                    .HasDefaultValue(true);

                entity.Property(r => r.IsDeleted)
                    .HasDefaultValue(false);

                entity.Property(r => r.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Composite Unique Constraint (Only for non-deleted records)
                entity.HasIndex(r => new { r.Source, r.Destination })
                    .IsUnique()
                    .HasFilter("[IsDeleted] = 0");

                // Query Filter for Soft Delete
                entity.HasQueryFilter(r => !r.IsDeleted);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.HasKey(s => s.ScheduleId);

                // 🔗 Foreign Key: Bus
                entity.HasOne(s => s.Bus)
                    .WithMany()
                    .HasForeignKey(s => s.BusId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 🔗 Foreign Key: Route
                entity.HasOne(s => s.Route)
                    .WithMany()
                    .HasForeignKey(s => s.RouteId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 💰 Decimal Precision
                //entity.Property(s => s.Fare)
                //    .HasColumnType("decimal(10,2)");

                // 🕒 Required Fields
                entity.Property(s => s.DepartureTime)
                    .IsRequired();

                entity.Property(s => s.ArrivalTime)
                    .IsRequired();

                entity.Property(s => s.IsActive)
                    .HasDefaultValue(true);

                entity.Property(s => s.IsDeleted)
                    .HasDefaultValue(false);

                entity.Property(s => s.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // 🔒 Composite Unique Constraint
                entity.HasIndex(s => new { s.BusId, s.RouteId, s.DepartureTime, s.TravelDate })
                    .IsUnique()
                    .HasFilter("[IsDeleted] = 0");

                // 🔍 Performance Indexes
                entity.HasIndex(s => s.BusId);
                entity.HasIndex(s => s.RouteId);
                entity.HasIndex(s => s.DepartureTime);
                entity.HasIndex(s => s.TravelDate);

                // 🧹 Soft Delete Query Filter
                entity.HasQueryFilter(s => !s.IsDeleted);
            });

        }
    }
}