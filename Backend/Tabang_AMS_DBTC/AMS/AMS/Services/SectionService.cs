using AMS.Data;
using AMS.DTOs;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace AMS.Services
{
    /// <summary>Business logic for section CRUD.</summary>
    public class SectionService : ISectionService
    {
        private readonly ApplicationDbContext _context;

        public SectionService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── GET ALL ────────────────────────────────────────────────────────
        public async Task<IEnumerable<SectionResponseDTO>> GetAllAsync()
        {
            var sections = await _context.Sections
                .Include(s => s.User)          // teacher info
                .Include(s => s.Students)      // to compute student count
                .OrderBy(s => s.Name)
                .ToListAsync();

            return sections.Select(MapToDto);
        }

        // ── GET BY ID ──────────────────────────────────────────────────────
        public async Task<SectionResponseDTO?> GetByIdAsync(int id)
        {
            var section = await _context.Sections
                .Include(s => s.User)
                .Include(s => s.Students)
                .FirstOrDefaultAsync(s => s.Id == id);

            return section is null ? null : MapToDto(section);
        }

        // ── GET BY TEACHER ─────────────────────────────────────────────────
        public async Task<IEnumerable<SectionResponseDTO>> GetByTeacherAsync(int userId)
        {
            var sections = await _context.Sections
                .Include(s => s.User)
                .Include(s => s.Students)
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return sections.Select(MapToDto);
        }

        // ── CREATE ─────────────────────────────────────────────────────────
        public async Task<SectionResponseDTO> CreateAsync(SectionDTO dto)
        {
            // Verify the teacher exists
            var teacher = await _context.Users.FindAsync(dto.UserId);
            if (teacher is null)
                throw new ArgumentException($"User (teacher) with ID {dto.UserId} not found.");

            var section = new Section
            {
                Name = dto.Name,
                SchoolYear = dto.SchoolYear,
                Semester = dto.Semester,
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Sections.Add(section);
            await _context.SaveChangesAsync();

            // Re-fetch with navigation properties
            return (await GetByIdAsync(section.Id))!;
        }

        // ── UPDATE ─────────────────────────────────────────────────────────
        public async Task<SectionResponseDTO?> UpdateAsync(int id, SectionDTO dto)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section is null) return null;

            // Verify new teacher if being changed
            if (dto.UserId != section.UserId)
            {
                var teacher = await _context.Users.FindAsync(dto.UserId);
                if (teacher is null)
                    throw new ArgumentException($"User (teacher) with ID {dto.UserId} not found.");
            }

            section.Name = dto.Name;
            section.SchoolYear = dto.SchoolYear;
            section.Semester = dto.Semester;
            section.UserId = dto.UserId;
            section.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        // ── DELETE ─────────────────────────────────────────────────────────
        public async Task<bool> DeleteAsync(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section is null) return false;

            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();
            return true;
        }

        // ── Mapping helper ─────────────────────────────────────────────────
        private static SectionResponseDTO MapToDto(Section s) => new()
        {
            Id = s.Id,
            Name = s.Name,
            SchoolYear = s.SchoolYear,
            Semester = s.Semester,
            UserId = s.UserId,
            TeacherName = s.User is not null
                ? $"{s.User.FirstName} {s.User.LastName}" : string.Empty,
            StudentCount = s.Students?.Count ?? 0,  // total enrolled students
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        };
    }
}