using AMS.Data;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.EntityFrameworkCore;

namespace AMS.Repositories
{
    // Concrete implementation of IStudentRepository using EF Core + PostgreSQL
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── Retrieve all students with their section info ──────────────
        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students
                .Include(s => s.Section) // include section for display purposes
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
        }

        // ── Retrieve a single student by PK ───────────────────────────
        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Section)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        // ── Retrieve all students in a specific section ────────────────
        public async Task<IEnumerable<Student>> GetBySectionIdAsync(int sectionId)
        {
            return await _context.Students
                .Include(s => s.Section)
                .Where(s => s.SectionId == sectionId)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
        }

        // ── Search students by partial name (case-insensitive) ─────────
        public async Task<IEnumerable<Student>> SearchByNameAsync(string name)
        {
            // EF Core translates .ToLower() to LOWER() in PostgreSQL
            var lowerName = name.ToLower();

            return await _context.Students
                .Include(s => s.Section)
                .Where(s => s.FirstName.ToLower().Contains(lowerName)
                         || s.LastName.ToLower().Contains(lowerName))
                .OrderBy(s => s.LastName)
                .ToListAsync();
        }

        // ── Insert a new student ───────────────────────────────────────
        public async Task<Student> CreateAsync(Student student)
        {
            student.CreatedAt = DateTime.UtcNow;
            student.UpdatedAt = DateTime.UtcNow;

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return student;
        }

        // ── Update an existing student record ──────────────────────────
        public async Task<Student> UpdateAsync(Student student)
        {
            student.UpdatedAt = DateTime.UtcNow;

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return student;
        }

        // ── Delete a student by ID ─────────────────────────────────────
        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student is null) return false;

            // Cascade delete will also remove the student's attendance records (configured in DbContext)
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}