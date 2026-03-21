using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMS.Models
{
    // Represents a class section (e.g., BSIT-2)
    [Table("sections")]
    public class Section
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // Section name, e.g., "BSIT-2"
        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        // Academic school year, e.g., "2025-2026"
        [Required]
        [MaxLength(20)]
        [Column("school_year")]
        public string SchoolYear { get; set; } = string.Empty;

        // Semester: "1st Semester" or "2nd Semester"
        [Required]
        [MaxLength(50)]
        [Column("semester")]
        public string Semester { get; set; } = string.Empty;

        // Foreign key — the teacher (user) assigned to this section
        [Column("user_id")]
        public int UserId { get; set; }

        // Navigation: the teacher who owns this section
        [ForeignKey("UserId")]
        public User? User { get; set; }

        // Timestamp when the record was created
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Timestamp when the record was last updated
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation: all students enrolled in this section
        public ICollection<Student> Students { get; set; } = new List<Student>();

        // Navigation: all attendance records for this section
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}