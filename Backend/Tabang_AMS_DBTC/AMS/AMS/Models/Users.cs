using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMS.Models
{
    // Represents a system user (Teacher or Admin)
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // User's first name
        [Required]
        [MaxLength(100)]
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        // User's last name
        [Required]
        [MaxLength(100)]
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        // Must be a valid email address used for login
        [Required]
        [MaxLength(200)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        // Hashed password — never store plain text
        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        // Role: "Teacher" or "Admin"
        [Required]
        [MaxLength(50)]
        [Column("role")]
        public string Role { get; set; } = "Teacher";

        // Timestamp when the record was created
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Timestamp when the record was last updated
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property — a user (teacher) can handle many sections
        public ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}