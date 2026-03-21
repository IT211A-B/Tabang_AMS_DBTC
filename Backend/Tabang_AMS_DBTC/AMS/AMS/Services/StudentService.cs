using AMS.Data;
using AMS.DTOs;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace AMS.Services
{
    /// <summary>Business logic for student CRUD and attendance summary.</summary>
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── GET ALL ────────────────────────────────────────────────────────
        public async Task<IEnumerable<StudentResponseDTO>> GetAllAsync()
        {
            var students = await _context.Students
                .Include(s => s.Section)
                .Include(s => s.Attendances) // for attendance summary counts
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();

            return students.Select(MapToDto);
        }

        // ── GET BY ID ──────────────────────────────────────────────────────
        public async Task<StudentResponseDTO?> GetByIdAsync(int id)
        {
            var student = await _context.Students
                .Include(s => s.Section)
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(s => s.Id == id);

            return student is null ? null : MapToDto(student);
        }

        // ── GET BY SECTION ─────────────────────────────────────────────────
        public async Task<IEnumerable<StudentResponseDTO>> GetBySectionAsync(int sectionId)
        {
            var students = await _context.Students
                .Include(s => s.Section)
                .Include(s => s.Attendances)
                .Where(s => s.SectionId == sectionId)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();

            return students.Select(MapToDto);
        }

        // ── SEARCH BY NAME ─────────────────────────────────────────────────
        public async Task<IEnumerable<StudentResponseDTO>> SearchAsync(string name)
        {
            // Case-insensitive partial match on first or last name
            var lower = name.ToLower();

            var students = await _context.Students
                .Include(s => s.Section)
                .Include(s => s.Attendances)
                .Where(s => s.FirstName.ToLower().Contains(lower)
                         || s.LastName.ToLower().Contains(lower))
                .OrderBy(s => s.LastName)
                .ToListAsync();

            return students.Select(MapToDto);
        }

        // ── CREATE ─────────────────────────────────────────────────────────
        public async Task<StudentResponseDTO> CreateAsync(StudentDTO dto)
        {
            // Validate section exists
            var section = await _context.Sections.FindAsync(dto.SectionId);
            if (section is null)
                throw new ArgumentException($"Section with ID {dto.SectionId} not found.");

            var student = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                StudentNumber = dto.StudentNumber,
                Email = dto.Email,
                SectionId = dto.SectionId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(student.Id))!;
        }

        // ── UPDATE ─────────────────────────────────────────────────────────
        public async Task<StudentResponseDTO?> UpdateAsync(int id, StudentDTO dto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student is null) return null;

            // Validate section if it's being changed
            if (dto.SectionId != student.SectionId)
            {
                var section = await _context.Sections.FindAsync(dto.SectionId);
                if (section is null)
                    throw new ArgumentException($"Section with ID {dto.SectionId} not found.");
            }

            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;
            student.StudentNumber = dto.StudentNumber;
            student.Email = dto.Email;
            student.SectionId = dto.SectionId;
            student.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        // ── DELETE ─────────────────────────────────────────────────────────
        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student is null) return false;

            // Cascade delete removes this student's attendance records (configured in DbContext)
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        // ── Mapping helper ─────────────────────────────────────────────────
        private static StudentResponseDTO MapToDto(Student s) => new()
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            StudentNumber = s.StudentNumber,
            Email = s.Email,
            SectionId = s.SectionId,
            SectionName = s.Section?.Name ?? string.Empty,
            // Compute attendance summary from loaded collection
            TotalPresent = s.Attendances.Count(a => a.Status == "Present"),
            TotalAbsent = s.Attendances.Count(a => a.Status == "Absent"),
            TotalLate = s.Attendances.Count(a => a.Status == "Late"),
            TotalExcused = s.Attendances.Count(a => a.Status == "Excused"),
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        };
    }
}