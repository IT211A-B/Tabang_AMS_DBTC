using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMS.Models
{
    // Represents a single attendance record for a student on a given date
    [Table("attendances")]
    public class Attendance
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // The date this attendance entry is for
        [Required]
        [Column("date")]
        public DateOnly Date { get; set; }

        // Status: "Present", "Absent", "Late", or "Excused"
        [Required]
        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "Present";

        // Optional remarks/notes from the teacher
        [MaxLength(500)]
        [Column("remarks")]
        public string? Remarks { get; set; }

        // Foreign key — which student this record belongs to
        [Column("student_id")]
        public int StudentId { get; set; }

        // Navigation: the student for this attendance record
        [ForeignKey("StudentId")]
        public Student? Student { get; set; }

        // Foreign key — which section's class meeting this record belongs to
        [Column("section_id")]
        public int SectionId { get; set; }

        // Navigation: the section for this attendance record
        [ForeignKey("SectionId")]
        public Section? Section { get; set; }

        // Timestamp when the record was created
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Timestamp when the record was last updated
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}