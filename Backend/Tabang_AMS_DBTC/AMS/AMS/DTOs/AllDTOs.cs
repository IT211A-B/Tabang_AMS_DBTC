using System.ComponentModel.DataAnnotations;

namespace AMS.DTOs
{
    // ══════════════════════════════════════════════════
    //  USER DTOs
    // ══════════════════════════════════════════════════

    /// <summary>Payload for creating a new user account.</summary>
    public class CreateUserDTO
    {
        [Required][MaxLength(100)] public string FirstName { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        // Plain-text password — hashed in the service before storage
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        // "Teacher" or "Admin"
        [Required] public string Role { get; set; } = "Teacher";
    }

    /// <summary>Payload for updating an existing user.</summary>
    public class UpdateUserDTO
    {
        [MaxLength(100)] public string? FirstName { get; set; }
        [MaxLength(100)] public string? LastName { get; set; }
        [EmailAddress][MaxLength(200)] public string? Email { get; set; }
        // Provide only when changing password; null = keep existing
        [MinLength(6)] public string? Password { get; set; }
        public string? Role { get; set; }
    }

    /// <summary>Returned to API consumers — never exposes password hash.</summary>
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


    // ══════════════════════════════════════════════════
    //  AUTH DTOs  (used by AuthService)
    // ══════════════════════════════════════════════════

    /// <summary>Payload for registering a new account.</summary>
    public class RegisterDTO
    {
        [Required][MaxLength(100)] public string FirstName { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required] public string Role { get; set; } = "Teacher";

        // Optional section name at registration time
        public string? Section { get; set; }
    }

    /// <summary>Payload for logging in.</summary>
    public class LoginDTO
    {
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }

    /// <summary>Returned after a successful login.</summary>
    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }


    // ══════════════════════════════════════════════════
    //  SECTION DTOs
    // ══════════════════════════════════════════════════

    /// <summary>Used when creating a new section.</summary>
    public class CreateSectionDTO
    {
        [Required][MaxLength(100)] public string Name { get; set; } = string.Empty;
        [Required][MaxLength(20)] public string SchoolYear { get; set; } = string.Empty;
        [Required][MaxLength(50)] public string Semester { get; set; } = string.Empty;
        // The teacher (user) assigned to manage this section
        [Required] public int UserId { get; set; }
    }

    /// <summary>Used when updating a section (all fields optional).</summary>
    public class UpdateSectionDTO
    {
        [MaxLength(100)] public string? Name { get; set; }
        [MaxLength(20)] public string? SchoolYear { get; set; }
        [MaxLength(50)] public string? Semester { get; set; }
        public int? UserId { get; set; }
    }

    /// <summary>Alias used by SectionService — same shape as CreateSectionDTO.</summary>
    public class SectionDTO
    {
        [Required][MaxLength(100)] public string Name { get; set; } = string.Empty;
        [Required][MaxLength(20)] public string SchoolYear { get; set; } = string.Empty;
        [Required][MaxLength(50)] public string Semester { get; set; } = string.Empty;
        [Required] public int UserId { get; set; }
    }

    /// <summary>Returned from section queries.</summary>
    public class SectionResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SchoolYear { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


    // ══════════════════════════════════════════════════
    //  STUDENT DTOs
    // ══════════════════════════════════════════════════

    /// <summary>Used when enrolling a new student.</summary>
    public class CreateStudentDTO
    {
        [Required][MaxLength(100)] public string FirstName { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string LastName { get; set; } = string.Empty;
        [MaxLength(50)] public string? StudentNumber { get; set; }
        [EmailAddress][MaxLength(200)] public string? Email { get; set; }
        [Required] public int SectionId { get; set; }
    }

    /// <summary>Used when updating a student record.</summary>
    public class UpdateStudentDTO
    {
        [MaxLength(100)] public string? FirstName { get; set; }
        [MaxLength(100)] public string? LastName { get; set; }
        [MaxLength(50)] public string? StudentNumber { get; set; }
        [EmailAddress][MaxLength(200)] public string? Email { get; set; }
        public int? SectionId { get; set; }
    }

    /// <summary>Alias used by StudentService — same shape as CreateStudentDTO.</summary>
    public class StudentDTO
    {
        [Required][MaxLength(100)] public string FirstName { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string LastName { get; set; } = string.Empty;
        [MaxLength(50)] public string? StudentNumber { get; set; }
        [EmailAddress][MaxLength(200)] public string? Email { get; set; }
        [Required] public int SectionId { get; set; }
    }

    /// <summary>Returned from student queries.</summary>
    public class StudentResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? StudentNumber { get; set; }
        public string? Email { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        // Attendance summary totals for dashboard cards
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalLate { get; set; }
        public int TotalExcused { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


    // ══════════════════════════════════════════════════
    //  ATTENDANCE DTOs
    // ══════════════════════════════════════════════════

    /// <summary>Used when recording new attendance.</summary>
    public class CreateAttendanceDTO
    {
        [Required] public DateOnly Date { get; set; }
        // Allowed: "Present", "Absent", "Late", "Excused"
        [Required][MaxLength(20)] public string Status { get; set; } = "Present";
        [MaxLength(500)] public string? Remarks { get; set; }
        [Required] public int StudentId { get; set; }
        [Required] public int SectionId { get; set; }
    }

    /// <summary>Used when correcting an attendance record (all fields optional).</summary>
    public class UpdateAttendanceDTO
    {
        [MaxLength(20)] public string? Status { get; set; }
        [MaxLength(500)] public string? Remarks { get; set; }
        public DateOnly? Date { get; set; }
    }

    /// <summary>Alias used by AttendanceService — same shape as CreateAttendanceDTO.</summary>
    public class AttendanceDTO
    {
        [Required] public DateOnly Date { get; set; }
        [Required][MaxLength(20)] public string Status { get; set; } = "Present";
        [MaxLength(500)] public string? Remarks { get; set; }
        [Required] public int StudentId { get; set; }
        [Required] public int SectionId { get; set; }
    }

    /// <summary>Returned from attendance queries.</summary>
    public class AttendanceResponseDTO
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


    // ══════════════════════════════════════════════════
    //  DASHBOARD DTOs
    // ══════════════════════════════════════════════════

    public class DashboardDTO
    {
        public int TotalStudents { get; set; }
        public int PresentToday { get; set; }
        public int AbsentToday { get; set; }
        public int LateToday { get; set; }
        public int ExcusedToday { get; set; }
        public Dictionary<string, int> WeeklyPresent { get; set; } = new();
        public Dictionary<string, int> WeeklyAbsent { get; set; } = new();
        public Dictionary<string, int> WeeklyLate { get; set; } = new();
        public List<AbsenceAlertDTO> AbsenceAlerts { get; set; } = new();
        public List<ActivityLogDTO> RecentActivity { get; set; } = new();
    }

    public class AbsenceAlertDTO
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int AbsenceCount { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ActivityLogDTO
    {
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    // ══════════════════════════════════════════════════
    //  TEACHER DTOs
    // ══════════════════════════════════════════════════

    public class CreateTeacherDTO
    {
        [Required][MaxLength(100)] public string FirstName { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)] public string? Phone { get; set; }
        [MaxLength(100)] public string? Department { get; set; }
    }

    public class UpdateTeacherDTO
    {
        [MaxLength(100)] public string? FirstName { get; set; }
        [MaxLength(100)] public string? LastName { get; set; }

        [EmailAddress, MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(20)] public string? Phone { get; set; }
        [MaxLength(100)] public string? Department { get; set; }
    }

    public class TeacherDTO
    {
        [Required][MaxLength(100)] public string FirstName { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)] public string? Phone { get; set; }
        [MaxLength(100)] public string? Department { get; set; }
    }

    public class TeacherResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Department { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}