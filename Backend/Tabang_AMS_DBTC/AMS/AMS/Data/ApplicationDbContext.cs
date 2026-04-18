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

        // ── DbSets (Tables) ───────────────────────────────────────
        public DbSet<User> Users { get; set; }
        public DbSet<Teacher> Teachers { get; set; }  
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

            // ── Teacher ────────────────────────────────────────────
            modelBuilder.Entity<Teacher>(entity =>
            {
                // Email must be unique
                entity.HasIndex(t => t.Email).IsUnique();

                entity.Property(t => t.FirstName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(t => t.LastName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(t => t.Email)
                      .IsRequired();

                entity.Property(t => t.Phone)
                      .HasMaxLength(20);

                entity.Property(t => t.Department)
                      .HasMaxLength(100);
            });

            // ── Section ────────────────────────────────────────────
            modelBuilder.Entity<Section>(entity =>
            {
                // A section belongs to one user (teacher)
                entity.HasOne(s => s.User)
                      .WithMany(u => u.Sections)
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // OPTIONAL: Section linked to Teacher table
                entity.HasOne(s => s.Teacher)
                      .WithMany()
                      .HasForeignKey(s => s.TeacherId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ── Student ────────────────────────────────────────────
            modelBuilder.Entity<Student>(entity =>
            {
                // A student belongs to one section
                entity.HasOne(st => st.Section)
                      .WithMany(s => s.Students)
                      .HasForeignKey(st => st.SectionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Attendance ─────────────────────────────────────────
            modelBuilder.Entity<Attendance>(entity =>
            {
                // Unique constraint per student per day per section
                entity.HasIndex(a => new { a.StudentId, a.SectionId, a.Date })
                      .IsUnique();

                // Attendance → Student
                entity.HasOne(a => a.Student)
                      .WithMany(st => st.Attendances)
                      .HasForeignKey(a => a.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Attendance → Section
                entity.HasOne(a => a.Section)
                      .WithMany(s => s.Attendances)
                      .HasForeignKey(a => a.SectionId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}