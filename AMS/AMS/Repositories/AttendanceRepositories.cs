using AMS.Data;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.EntityFrameworkCore;

namespace AMS.Repositories
{
    // Concrete implementation of IAttendanceRepository using EF Core + PostgreSQL
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── Retrieve all attendance records (with related data) ────────
        public async Task<IEnumerable<Attendance>> GetAllAsync()
        {
            return await _context.Attendances
                .Include(a => a.Student)  // student name
                .Include(a => a.Section)  // section name
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        // ── Retrieve a single attendance record by PK ──────────────────
        public async Task<Attendance?> GetByIdAsync(int id)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Section)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // ── Retrieve all records for a specific student ────────────────
        public async Task<IEnumerable<Attendance>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Attendances
                .Include(a => a.Section)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        // ── Retrieve all records for a section on a given date ─────────
        // Typically used when marking daily attendance for the whole class
        public async Task<IEnumerable<Attendance>> GetBySectionAndDateAsync(
            int sectionId, DateOnly date)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.SectionId == sectionId && a.Date == date)
                .OrderBy(a => a.Student!.LastName)
                .ToListAsync();
        }

        // ── Retrieve records for a section within a date range ─────────
        // Useful for generating weekly/monthly attendance reports
        public async Task<IEnumerable<Attendance>> GetBySectionAndDateRangeAsync(
            int sectionId, DateOnly from, DateOnly to)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.SectionId == sectionId
                         && a.Date >= from
                         && a.Date <= to)
                .OrderBy(a => a.Date)
                .ThenBy(a => a.Student!.LastName)
                .ToListAsync();
        }

        // ── Check for duplicate record (student + section + date) ──────
        public async Task<bool> ExistsAsync(int studentId, int sectionId, DateOnly date)
        {
            return await _context.Attendances
                .AnyAsync(a => a.StudentId == studentId
                            && a.SectionId == sectionId
                            && a.Date == date);
        }

        // ── Insert a new attendance record ─────────────────────────────
        public async Task<Attendance> CreateAsync(Attendance attendance)
        {
            attendance.CreatedAt = DateTime.UtcNow;
            attendance.UpdatedAt = DateTime.UtcNow;

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return attendance;
        }

        // ── Update an existing attendance record ───────────────────────
        public async Task<Attendance> UpdateAsync(Attendance attendance)
        {
            attendance.UpdatedAt = DateTime.UtcNow;

            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();

            return attendance;
        }

        // ── Delete an attendance record by ID ──────────────────────────
        public async Task<bool> DeleteAsync(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance is null) return false;

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}