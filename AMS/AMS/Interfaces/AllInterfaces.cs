using AMS.DTOs;
using AMS.Models;

namespace AMS.Interfaces
{
    // ─────────────────────────────────────────────────────────────────────────
    //  Generic repository — standard CRUD contract for all entities
    // ─────────────────────────────────────────────────────────────────────────
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();     // all records
        Task<T?> GetByIdAsync(int id);          // single record by PK
        Task<T> CreateAsync(T entity);          // insert
        Task<T> UpdateAsync(T entity);          // update
        Task<bool> DeleteAsync(int id);         // delete; false = not found
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  USER repository
    // ─────────────────────────────────────────────────────────────────────────
    public interface IUserRepository : IRepository<User>
    {
        // Look up by email — used for login and uniqueness checks
        Task<User?> GetByEmailAsync(string email);
        // All users with a specific role ("Teacher" | "Admin")
        Task<IEnumerable<User>> GetByRoleAsync(string role);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  SECTION repository
    // ─────────────────────────────────────────────────────────────────────────
    public interface ISectionRepository : IRepository<Section>
    {
        // All sections belonging to one teacher
        Task<IEnumerable<Section>> GetByUserIdAsync(int userId);
        // Section with students eagerly loaded (avoids N+1)
        Task<Section?> GetWithStudentsAsync(int sectionId);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  STUDENT repository
    // ─────────────────────────────────────────────────────────────────────────
    public interface IStudentRepository : IRepository<Student>
    {
        // All students enrolled in a section
        Task<IEnumerable<Student>> GetBySectionIdAsync(int sectionId);
        // Case-insensitive partial name search
        Task<IEnumerable<Student>> SearchByNameAsync(string name);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  ATTENDANCE repository
    // ─────────────────────────────────────────────────────────────────────────
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        // All records for one student
        Task<IEnumerable<Attendance>> GetByStudentIdAsync(int studentId);
        // All records for a section on a specific date
        Task<IEnumerable<Attendance>> GetBySectionAndDateAsync(int sectionId, DateOnly date);
        // All records for a section within a date range
        Task<IEnumerable<Attendance>> GetBySectionAndDateRangeAsync(
            int sectionId, DateOnly from, DateOnly to);
        // Duplicate check — one record per student per section per day
        Task<bool> ExistsAsync(int studentId, int sectionId, DateOnly date);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  AUTH service
    // ─────────────────────────────────────────────────────────────────────────
    public interface IAuthService
    {
        Task<UserResponseDTO> RegisterAsync(RegisterDTO dto);
        // Returns null when credentials are invalid
        Task<AuthResponseDTO?> LoginAsync(LoginDTO dto);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  ATTENDANCE service
    // ─────────────────────────────────────────────────────────────────────────
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceResponseDTO>> GetAllAsync();
        Task<AttendanceResponseDTO?> GetByIdAsync(int id);
        Task<IEnumerable<AttendanceResponseDTO>> GetByStudentAsync(int studentId);
        Task<IEnumerable<AttendanceResponseDTO>> GetByDateAsync(int sectionId, DateOnly date);
        Task<IEnumerable<AttendanceResponseDTO>> GetByRangeAsync(
            int sectionId, DateOnly from, DateOnly to);
        // Returns null on duplicate
        Task<AttendanceResponseDTO?> CreateAsync(AttendanceDTO dto);
        Task<AttendanceResponseDTO?> UpdateAsync(int id, AttendanceDTO dto);
        Task<bool> DeleteAsync(int id);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  SECTION service
    // ─────────────────────────────────────────────────────────────────────────
    public interface ISectionService
    {
        Task<IEnumerable<SectionResponseDTO>> GetAllAsync();
        Task<SectionResponseDTO?> GetByIdAsync(int id);
        Task<IEnumerable<SectionResponseDTO>> GetByTeacherAsync(int userId);
        Task<SectionResponseDTO> CreateAsync(SectionDTO dto);
        Task<SectionResponseDTO?> UpdateAsync(int id, SectionDTO dto);
        Task<bool> DeleteAsync(int id);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  STUDENT service
    // ─────────────────────────────────────────────────────────────────────────
    public interface IStudentService
    {
        Task<IEnumerable<StudentResponseDTO>> GetAllAsync();
        Task<StudentResponseDTO?> GetByIdAsync(int id);
        Task<IEnumerable<StudentResponseDTO>> GetBySectionAsync(int sectionId);
        Task<IEnumerable<StudentResponseDTO>> SearchAsync(string name);
        Task<StudentResponseDTO> CreateAsync(StudentDTO dto);
        Task<StudentResponseDTO?> UpdateAsync(int id, StudentDTO dto);
        Task<bool> DeleteAsync(int id);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  DASHBOARD service
    // ─────────────────────────────────────────────────────────────────────────
    public interface IDashboardService
    {
        // Returns today's stats, weekly trends, and alerts for a section
        Task<DashboardDTO> GetDashboardAsync(int sectionId);
    }
}