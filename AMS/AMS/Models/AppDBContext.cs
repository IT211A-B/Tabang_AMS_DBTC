using Microsoft.EntityFrameworkCore;

namespace AMS.Models
{
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options)
            {
            }

            public DbSet<Student> Students { get; set; }

            public DbSet<Course> Courses { get; set; }

            public DbSet<User> Users { get; set; }

            public DbSet<Attendance> Attendances { get; set; }
        }
}