using AMS.Data;
using AMS.DTOs;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.EntityFrameworkCore;

namespace AMS.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _context;

        public TeacherService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TeacherResponseDTO>> GetAllAsync()
        {
            var teachers = await _context.Teachers
                .OrderBy(t => t.LastName)
                .ThenBy(t => t.FirstName)
                .ToListAsync();

            return teachers.Select(Map);
        }

        public async Task<TeacherResponseDTO?> GetByIdAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            return teacher is null ? null : Map(teacher);
        }

        public async Task<TeacherResponseDTO?> GetByEmailAsync(string email)
        {
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Email.ToLower() == email.ToLower());

            return teacher is null ? null : Map(teacher);
        }

        public async Task<TeacherResponseDTO> CreateAsync(CreateTeacherDTO dto)
        {
            var exists = await _context.Teachers
                .AnyAsync(t => t.Email.ToLower() == dto.Email.ToLower());

            if (exists)
                throw new ArgumentException("Email already exists.");

            var teacher = new Teacher
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Department = dto.Department,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return Map(teacher);
        }

        public async Task<TeacherResponseDTO?> UpdateAsync(int id, UpdateTeacherDTO dto)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher is null) return null;

            if (!string.IsNullOrWhiteSpace(dto.FirstName)) teacher.FirstName = dto.FirstName;
            if (!string.IsNullOrWhiteSpace(dto.LastName)) teacher.LastName = dto.LastName;

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var exists = await _context.Teachers
                    .AnyAsync(t => t.Email.ToLower() == dto.Email.ToLower() && t.Id != id);

                if (exists)
                    throw new ArgumentException("Email already used.");

                teacher.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Phone)) teacher.Phone = dto.Phone;
            if (!string.IsNullOrWhiteSpace(dto.Department)) teacher.Department = dto.Department;

            teacher.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Map(teacher);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher is null) return false;

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return true;
        }

        private static TeacherResponseDTO Map(Teacher t) => new()
        {
            Id = t.Id,
            FirstName = t.FirstName,
            LastName = t.LastName,
            Email = t.Email,
            Phone = t.Phone,
            Department = t.Department,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt ?? DateTime.UtcNow
        };
    }
}