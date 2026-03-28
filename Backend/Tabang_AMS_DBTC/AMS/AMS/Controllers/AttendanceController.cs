using AMS.DTOs;
using AMS.Interfaces;
using AMS.Models;
using AMS.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    /// <summary>
    /// Manages attendance records for students within sections.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly ISectionRepository _sectionRepo;

    public AttendanceController(
        IAttendanceRepository attendanceRepo,
        IStudentRepository studentRepo,
        ISectionRepository sectionRepo)
        {
            _attendanceRepo = attendanceRepo;
            _studentRepo = studentRepo;
            _sectionRepo = sectionRepo;
        }

        /// <summary>Returns paginated attendance records.</summary>
        /// <param name="pageNumber">Page number (default = 1)</param>
        /// <param name="pageSize">Number of records per page (default = 10)</param>
        [HttpGet]
        public async Task<ActionResult<object>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var records = await _attendanceRepo.GetAllAsync();

            var totalRecords = records.Count();

            var paginatedRecords = records
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDTO);

            var result = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                Data = paginatedRecords
            };

            return Ok(result);
        }

        /// <summary>Gets a single attendance record by ID.</summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AttendanceResponseDTO>> GetById(int id)
        {
            var record = await _attendanceRepo.GetByIdAsync(id);
            if (record is null) return NotFound($"Attendance record with ID {id} not found.");
            return Ok(MapToDTO(record));
        }

        /// <summary>Returns the full attendance history for a specific student.</summary>
        [HttpGet("student/{studentId:int}")]
        public async Task<ActionResult<IEnumerable<AttendanceResponseDTO>>> GetByStudent(int studentId)
        {
            var records = await _attendanceRepo.GetByStudentIdAsync(studentId);
            return Ok(records.Select(MapToDTO));
        }

        /// <summary>Returns all attendance records for a section on a specific date.</summary>
        [HttpGet("section/{sectionId:int}/date/{date}")]
        public async Task<ActionResult<IEnumerable<AttendanceResponseDTO>>> GetBySectionAndDate(
            int sectionId, DateOnly date)
        {
            var records = await _attendanceRepo.GetBySectionAndDateAsync(sectionId, date);
            return Ok(records.Select(MapToDTO));
        }

        /// <summary>Returns attendance records for a section within a date range.</summary>
        [HttpGet("section/{sectionId:int}/range")]
        public async Task<ActionResult<IEnumerable<AttendanceResponseDTO>>> GetByRange(
            int sectionId,
            [FromQuery] DateOnly from,
            [FromQuery] DateOnly to)
        {
            if (from > to)
                return BadRequest("'from' date must be on or before 'to' date.");

            var records = await _attendanceRepo.GetBySectionAndDateRangeAsync(sectionId, from, to);
            return Ok(records.Select(MapToDTO));
        }

        /// <summary>Records new attendance for a student in a section.</summary>
        [HttpPost]
        public async Task<ActionResult<AttendanceResponseDTO>> Create(
            [FromBody] CreateAttendanceDTO dto)
        {
            var student = await _studentRepo.GetByIdAsync(dto.StudentId);
            if (student is null)
                return BadRequest($"Student with ID {dto.StudentId} does not exist.");

            var section = await _sectionRepo.GetByIdAsync(dto.SectionId);
            if (section is null)
                return BadRequest($"Section with ID {dto.SectionId} does not exist.");

            var duplicate = await _attendanceRepo.ExistsAsync(dto.StudentId, dto.SectionId, dto.Date);
            if (duplicate)
                return Conflict(
                    $"Attendance for student {dto.StudentId} in section {dto.SectionId} on {dto.Date} already exists.");

            var validStatuses = new[] { "Present", "Absent", "Late", "Excused" };
            if (!validStatuses.Contains(dto.Status))
                return BadRequest($"Invalid status '{dto.Status}'. Allowed: {string.Join(", ", validStatuses)}");

            var entity = new Attendance
            {
                Date = dto.Date,
                Status = dto.Status,
                Remarks = dto.Remarks,
                StudentId = dto.StudentId,
                SectionId = dto.SectionId
            };

            var created = await _attendanceRepo.CreateAsync(entity);
            var withNav = await _attendanceRepo.GetByIdAsync(created.Id);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDTO(withNav!));
        }

        /// <summary>Updates an existing attendance record.</summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<AttendanceResponseDTO>> Update(
            int id, [FromBody] UpdateAttendanceDTO dto)
        {
            var record = await _attendanceRepo.GetByIdAsync(id);
            if (record is null) return NotFound($"Attendance record with ID {id} not found.");

            if (dto.Status is not null)
            {
                var validStatuses = new[] { "Present", "Absent", "Late", "Excused" };
                if (!validStatuses.Contains(dto.Status))
                    return BadRequest($"Invalid status '{dto.Status}'.");

                record.Status = dto.Status;
            }

            if (dto.Remarks is not null) record.Remarks = dto.Remarks;
            if (dto.Date.HasValue) record.Date = dto.Date.Value;

            await _attendanceRepo.UpdateAsync(record);

            var updated = await _attendanceRepo.GetByIdAsync(id);
            return Ok(MapToDTO(updated!));
        }

        /// <summary>Deletes an attendance record by ID.</summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _attendanceRepo.DeleteAsync(id);
            if (!deleted) return NotFound($"Attendance record with ID {id} not found.");
            return NoContent();
        }

        private static AttendanceResponseDTO MapToDTO(Attendance a) => new()
        {
            Id = a.Id,
            Date = a.Date,
            Status = a.Status,
            Remarks = a.Remarks,
            StudentId = a.StudentId,
            StudentName = a.Student is not null
                            ? $"{a.Student.FirstName} {a.Student.LastName}"
                            : string.Empty,
            SectionId = a.SectionId,
            SectionName = a.Section?.Name ?? string.Empty,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        };
    }
}
