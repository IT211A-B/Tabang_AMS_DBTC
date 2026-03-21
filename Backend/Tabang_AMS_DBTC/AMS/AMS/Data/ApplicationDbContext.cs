using AMS.Models;
using Microsoft.EntityFrameworkCore;

namespace AMS.Data
{
    // EF Core DbContext configured for PostgreSQL via Npgsql
    public class ApplicationDbContext : DbContext
    {
        // Constructor receives options (connection string etc.) via DI
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // DbSet for each entity — maps to corresponding PostgreSQL tables
        public DbSet<User> Users { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── User ───────────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                // Email must be unique across all users
                entity.HasIndex(u => u.Email).IsUnique();

                // Role is constrained to Teacher or Admin
                entity.Property(u => u.Role)
                      .HasDefaultValue("Teacher");
            });

            // ── Section ────────────────────────────────────────────
            modelBuilder.Entity<Section>(entity =>
            {
                // A section belongs to one user (teacher)
                entity.HasOne(s => s.User)
                      .WithMany(u => u.Sections)
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Restrict); // prevent cascade-deleting teacher's sections
            });

            // ── Student ────────────────────────────────────────────
            modelBuilder.Entity<Student>(entity =>
            {
                // A student belongs to one section
                entity.HasOne(st => st.Section)
                      .WithMany(s => s.Students)
                      .HasForeignKey(st => st.SectionId)
                      .OnDelete(DeleteBehavior.Cascade); // removing a section removes its students
            });

            // ── Attendance ─────────────────────────────────────────
            modelBuilder.Entity<Attendance>(entity =>
            {
                // One student should have at most one attendance record per section per date
                entity.HasIndex(a => new { a.StudentId, a.SectionId, a.Date })
                      .IsUnique();

                // Attendance links to a student
                entity.HasOne(a => a.Student)
                      .WithMany(st => st.Attendances)
                      .HasForeignKey(a => a.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Attendance links to a section
                entity.HasOne(a => a.Section)
                      .WithMany(s => s.Attendances)
                      .HasForeignKey(a => a.SectionId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}