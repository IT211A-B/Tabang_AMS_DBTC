using AMS.DTOs;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    /// <summary>
    /// Manage student attendance records.
    /// Supports pagination and searching.
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

        /// <summary>
        /// Get attendance records with pagination and optional search filters.
        /// </summary>
        /// <param name="studentId">Filter by student ID</param>
        /// <param name="sectionId">Filter by section ID</param>
        /// <param name="status">Filter by attendance status (Present, Absent, Late, Excused)</param>
        /// <param name="date">Filter by specific date</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Records per page (default: 10)</param>
        [HttpGet]
        public async Task<ActionResult<object>> Get(
            [FromQuery] int? studentId,
            [FromQuery] int? sectionId,
            [FromQuery] string? status,
            [FromQuery] DateOnly? date,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var records = await _attendanceRepo.GetAllAsync();

            if (studentId.HasValue)
                records = records.Where(x => x.StudentId == studentId);

            if (sectionId.HasValue)
                records = records.Where(x => x.SectionId == sectionId);

            if (!string.IsNullOrWhiteSpace(status))
                records = records.Where(x => x.Status == status);

            if (date.HasValue)
                records = records.Where(x => x.Date == date);

            var total = records.Count();

            var data = records
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDTO);

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                Data = data
            });
        }

        /// <summary>
        /// Create a new attendance record.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AttendanceResponseDTO>> Post(CreateAttendanceDTO dto)
        {
            if (dto.StudentId <= 0 || dto.SectionId <= 0)
                return BadRequest("StudentId and SectionId are required.");

            var student = await _studentRepo.GetByIdAsync(dto.StudentId);
            if (student is null)
                return BadRequest("Student does not exist.");

            var section = await _sectionRepo.GetByIdAsync(dto.SectionId);
            if (section is null)
                return BadRequest("Section does not exist.");

            var validStatus = new[] { "Present", "Absent", "Late", "Excused" };
            if (!validStatus.Contains(dto.Status))
                return BadRequest("Invalid attendance status.");

            var entity = new Attendance
            {
                StudentId = dto.StudentId,
                SectionId = dto.SectionId,
                Date = dto.Date,
                Status = dto.Status,
                Remarks = dto.Remarks
            };

            var created = await _attendanceRepo.CreateAsync(entity);
            var result = await _attendanceRepo.GetByIdAsync(created.Id);

            return Ok(MapToDTO(result!));
        }

        /// <summary>
        /// Update an existing attendance record.
        /// </summary>
        /// <param name="id">Attendance ID</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdateAttendanceDTO dto)
        {
            var record = await _attendanceRepo.GetByIdAsync(id);
            if (record is null)
                return NotFound("Attendance record not found.");

            if (dto.Status is not null)
            {
                var validStatus = new[] { "Present", "Absent", "Late", "Excused" };
                if (!validStatus.Contains(dto.Status))
                    return BadRequest("Invalid attendance status.");

                record.Status = dto.Status;
            }

            if (dto.Date.HasValue)
                record.Date = dto.Date.Value;

            if (dto.Remarks != null)
                record.Remarks = dto.Remarks;

            await _attendanceRepo.UpdateAsync(record);

            return Ok(MapToDTO(record));
        }

        /// <summary>
        /// Delete an attendance record.
        /// </summary>
        /// <param name="id">Attendance ID</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _attendanceRepo.DeleteAsync(id);

            if (!deleted)
                return NotFound("Attendance record not found.");

            return NoContent();
        }

        /// <summary>
        /// Maps Attendance entity to DTO.
        /// </summary>
        private static AttendanceResponseDTO MapToDTO(Attendance a) => new()
        {
            Id = a.Id,
            Date = a.Date,
            Status = a.Status,
            Remarks = a.Remarks,
            StudentId = a.StudentId,
            StudentName = a.Student != null ? $"{a.Student.FirstName} {a.Student.LastName}" : "",
            SectionId = a.SectionId,
            SectionName = a.Section?.Name ?? "",
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        };
    }
}