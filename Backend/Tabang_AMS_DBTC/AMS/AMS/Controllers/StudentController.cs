using AMS.DTOs;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    /// <summary>
    /// API for managing students and their section enrollment.
    /// Supports searching and pagination.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _studentRepo;
        private readonly ISectionRepository _sectionRepo;

        public StudentsController(IStudentRepository studentRepo, ISectionRepository sectionRepo)
        {
            _studentRepo = studentRepo;
            _sectionRepo = sectionRepo;
        }

        /// <summary>
        /// Get students with pagination and optional search filters.
        /// </summary>
        /// <param name="name">Search by first or last name</param>
        /// <param name="sectionId">Filter by section</param>
        /// <param name="page">Page number (default = 1)</param>
        /// <param name="pageSize">Number of records per page (default = 10)</param>
        /// <response code="200">Returns paginated students</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentResponseDTO>>> Get(
            [FromQuery] string? name = null,
            [FromQuery] int? sectionId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Page and pageSize must be greater than 0.");

            var students = await _studentRepo.GetAllAsync();

            // Filtering
            if (!string.IsNullOrWhiteSpace(name))
                students = students.Where(s =>
                    (s.FirstName + " " + s.LastName)
                    .Contains(name, StringComparison.OrdinalIgnoreCase));

            if (sectionId.HasValue)
                students = students.Where(s => s.SectionId == sectionId);

            // Pagination
            var result = students
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDTO);

            return Ok(result);
        }

        /// <summary>
        /// Create a new student.
        /// </summary>
        /// <remarks>
        /// Example request:
        /// 
        /// POST /api/students
        /// 
        /// {
        ///   "FirstName": "Juan",
        ///   "LastName": "Dela Cruz",
        ///   "StudentNumber": "01",
        ///   "Email": "juan@email.com",
        ///   "SectionId": 1
        /// }
        /// </remarks>
        /// <response code="201">Student successfully created</response>
        /// <response code="400">Invalid input or section not found</response>
        [HttpPost]
        public async Task<ActionResult<StudentResponseDTO>> Post([FromBody] CreateStudentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var section = await _sectionRepo.GetByIdAsync(dto.SectionId);
            if (section is null)
                return BadRequest("Invalid SectionId.");

            var student = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                StudentNumber = dto.StudentNumber,
                Email = dto.Email,
                SectionId = dto.SectionId
            };

            var created = await _studentRepo.CreateAsync(student);

            return CreatedAtAction(nameof(Get),
                new { id = created.Id },
                MapToDTO(created));
        }

        /// <summary>
        /// Update an existing student.
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <response code="200">Student updated</response>
        /// <response code="404">Student not found</response>
        /// <response code="400">Invalid data</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<StudentResponseDTO>> Put(int id, [FromBody] UpdateStudentDTO dto)
        {
            var student = await _studentRepo.GetByIdAsync(id);
            if (student == null)
                return NotFound("Student not found.");

            if (dto.SectionId.HasValue)
            {
                var section = await _sectionRepo.GetByIdAsync(dto.SectionId.Value);
                if (section == null)
                    return BadRequest("Invalid SectionId.");

                student.SectionId = dto.SectionId.Value;
            }

            student.FirstName = dto.FirstName ?? student.FirstName;
            student.LastName = dto.LastName ?? student.LastName;
            student.Email = dto.Email ?? student.Email;
            student.StudentNumber = dto.StudentNumber ?? student.StudentNumber;

            await _studentRepo.UpdateAsync(student);

            return Ok(MapToDTO(student));
        }

        /// <summary>
        /// Delete a student by ID.
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <response code="204">Student deleted</response>
        /// <response code="404">Student not found</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _studentRepo.DeleteAsync(id);

            if (!deleted)
                return NotFound("Student not found.");

            return NoContent();
        }

        /// <summary>
        /// Converts Student entity to DTO response.
        /// </summary>
        private static StudentResponseDTO MapToDTO(Student s) => new()
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            StudentNumber = s.StudentNumber,
            Email = s.Email,
            SectionId = s.SectionId,
            SectionName = s.Section?.Name ?? "",
            TotalPresent = s.Attendances.Count(a => a.Status == "Present"),
            TotalAbsent = s.Attendances.Count(a => a.Status == "Absent"),
            TotalLate = s.Attendances.Count(a => a.Status == "Late"),
            TotalExcused = s.Attendances.Count(a => a.Status == "Excused"),
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        };
    }
}