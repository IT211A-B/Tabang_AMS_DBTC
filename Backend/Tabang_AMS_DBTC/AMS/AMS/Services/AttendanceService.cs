using AMS.Data;
using AMS.DTOs;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace AMS.Services
{
    /// <summary>
    /// Business logic for attendance CRUD and reporting queries.
    /// </summary>
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;

        public AttendanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Valid status values — enforced before any write
        private static readonly string[] ValidStatuses = { "Present", "Absent", "Late", "Excused" };

        // ── GET ALL ────────────────────────────────────────────────────────
        public async Task<IEnumerable<AttendanceResponseDTO>> GetAllAsync()
        {
            var records = await _context.Attendances
                .Include(a => a.Student)   // for student name
                .Include(a => a.Section)   // for section name
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return records.Select(MapToDto);
        }

        // ── GET BY ID ──────────────────────────────────────────────────────
        public async Task<AttendanceResponseDTO?> GetByIdAsync(int id)
        {
            var record = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Section)
                .FirstOrDefaultAsync(a => a.Id == id);

            return record is null ? null : MapToDto(record);
        }

        // ── GET BY STUDENT ─────────────────────────────────────────────────
        public async Task<IEnumerable<AttendanceResponseDTO>> GetByStudentAsync(int studentId)
        {
            var records = await _context.Attendances
                .Include(a => a.Section)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return records.Select(MapToDto);
        }

        // ── GET BY DATE (used in GetByDate / existing service reference) ───
        public async Task<IEnumerable<AttendanceResponseDTO>> GetByDateAsync(
            int sectionId, DateOnly date)
        {
            var records = await _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.SectionId == sectionId && a.Date == date)
                .OrderBy(a => a.Student!.LastName)
                .ToListAsync();

            return records.Select(MapToDto);
        }

        // ── GET BY DATE RANGE ──────────────────────────────────────────────
        public async Task<IEnumerable<AttendanceResponseDTO>> GetByRangeAsync(
            int sectionId, DateOnly from, DateOnly to)
        {
            var records = await _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.SectionId == sectionId
                         && a.Date >= from
                         && a.Date <= to)
                .OrderBy(a => a.Date)
                .ThenBy(a => a.Student!.LastName)
                .ToListAsync();

            return records.Select(MapToDto);
        }

        // ── CREATE ─────────────────────────────────────────────────────────
        public async Task<AttendanceResponseDTO?> CreateAsync(AttendanceDTO dto)
        {
            // Reject invalid status values early
            if (!ValidStatuses.Contains(dto.Status))
                throw new ArgumentException($"Invalid status '{dto.Status}'.");

            // Prevent duplicate record for the same student + section + date
            var duplicate = await _context.Attendances.AnyAsync(
                a => a.StudentId == dto.StudentId
                  && a.SectionId == dto.SectionId
                  && a.Date == dto.Date);

            if (duplicate) return null; // caller should respond with 409 Conflict

            var entity = new Attendance
            {
                Date = dto.Date,
                Status = dto.Status,
                Remarks = dto.Remarks,
                StudentId = dto.StudentId,
                SectionId = dto.SectionId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Attendances.Add(entity);
            await _context.SaveChangesAsync();

            // Re-fetch with navigation properties for the response
            return await GetByIdAsync(entity.Id);
        }

        // ── UPDATE ─────────────────────────────────────────────────────────
        public async Task<AttendanceResponseDTO?> UpdateAsync(int id, AttendanceDTO dto)
        {
            var entity = await _context.Attendances.FindAsync(id);
            if (entity is null) return null;

            // Validate status if provided
            if (!ValidStatuses.Contains(dto.Status))
                throw new ArgumentException($"Invalid status '{dto.Status}'.");

            // Apply updates
            entity.Date = dto.Date;
            entity.Status = dto.Status;
            entity.Remarks = dto.Remarks;
            entity.StudentId = dto.StudentId;
            entity.SectionId = dto.SectionId;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        // ── DELETE ─────────────────────────────────────────────────────────
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Attendances.FindAsync(id);
            if (entity is null) return false;

            _context.Attendances.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // ── Mapping helper ─────────────────────────────────────────────────
        private static AttendanceResponseDTO MapToDto(Attendance a) => new()
        {
            Id = a.Id,
            Date = a.Date,
            Status = a.Status,
            Remarks = a.Remarks,
            StudentId = a.StudentId,
            StudentName = a.Student is not null
                ? $"{a.Student.FirstName} {a.Student.LastName}" : string.Empty,
            SectionId = a.SectionId,
            SectionName = a.Section?.Name ?? string.Empty,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        };
    }
}