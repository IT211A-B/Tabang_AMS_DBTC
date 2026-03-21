using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMS.Models
{
    // Represents a student enrolled in a section
    [Table("students")]
    public class Student
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // Student's first name
        [Required]
        [MaxLength(100)]
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        // Student's last name
        [Required]
        [MaxLength(100)]
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        // Optional student ID number (school-issued)
        [MaxLength(50)]
        [Column("student_number")]
        public string? StudentNumber { get; set; }

        // Optional contact email for the student
        [MaxLength(200)]
        [Column("email")]
        public string? Email { get; set; }

        // Foreign key — which section this student belongs to
        [Column("section_id")]
        public int SectionId { get; set; }

        // Navigation: the section this student is enrolled in
        [ForeignKey("SectionId")]
        public Section? Section { get; set; }

        // Timestamp when the record was created
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Timestamp when the record was last updated
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation: all attendance records for this student
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}