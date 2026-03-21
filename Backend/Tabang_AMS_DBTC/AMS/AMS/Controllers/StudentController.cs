using AMS.DTOs;
using AMS.Interfaces;
using AMS.Models;
using AMS.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    /// <summary>
    /// Manages student enrollment and records across sections.
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

        /// <summary>Returns all students across all sections.</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentResponseDTO>>> GetAll()
        {
            var students = await _studentRepo.GetAllAsync();
            return Ok(students.Select(MapToDTO));
        }

        /// <summary>Gets a single student by ID.</summary>
        /// <param name="id">The student ID.</param>
        /// <response code="200">Student found.</response>
        /// <response code="404">No student with that ID.</response>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<StudentResponseDTO>> GetById(int id)
        {
            var student = await _studentRepo.GetByIdAsync(id);
            if (student is null) return NotFound($"Student with ID {id} not found.");
            return Ok(MapToDTO(student));
        }

        /// <summary>Returns all students enrolled in a specific section.</summary>
        /// <param name="sectionId">The section ID.</param>
        [HttpGet("section/{sectionId:int}")]
        public async Task<ActionResult<IEnumerable<StudentResponseDTO>>> GetBySection(int sectionId)
        {
            var students = await _studentRepo.GetBySectionIdAsync(sectionId);
            return Ok(students.Select(MapToDTO));
        }

        /// <summary>Case-insensitive partial name search across all students.</summary>
        /// <param name="name">Partial or full name to search for.</param>
        /// <response code="200">Matching students returned.</response>
        /// <response code="400">The name query parameter is missing or empty.</response>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<StudentResponseDTO>>> Search(
            [FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Query parameter 'name' is required.");

            var students = await _studentRepo.SearchByNameAsync(name);
            return Ok(students.Select(MapToDTO));
        }

        /// <summary>Enrolls a new student into a section.</summary>
        /// <response code="201">Student created and enrolled successfully.</response>
        /// <response code="400">The provided section ID does not exist.</response>
        [HttpPost]
        public async Task<ActionResult<StudentResponseDTO>> Create(
            [FromBody] CreateStudentDTO dto)
        {
            var section = await _sectionRepo.GetByIdAsync(dto.SectionId);
            if (section is null)
                return BadRequest($"Section with ID {dto.SectionId} does not exist.");

            var student = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                StudentNumber = dto.StudentNumber,
                Email = dto.Email,
                SectionId = dto.SectionId
            };

            var created = await _studentRepo.CreateAsync(student);
            var withSection = await _studentRepo.GetByIdAsync(created.Id);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDTO(withSection!));
        }

        /// <summary>Partially or fully updates a student record.</summary>
        /// <param name="id">The student ID.</param>
        /// <response code="200">Student updated successfully.</response>
        /// <response code="400">The provided section ID does not exist.</response>
        /// <response code="404">No student with that ID.</response>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<StudentResponseDTO>> Update(
            int id, [FromBody] UpdateStudentDTO dto)
        {
            var student = await _studentRepo.GetByIdAsync(id);
            if (student is null) return NotFound($"Student with ID {id} not found.");

            if (dto.FirstName is not null) student.FirstName = dto.FirstName;
            if (dto.LastName is not null) student.LastName = dto.LastName;
            if (dto.StudentNumber is not null) student.StudentNumber = dto.StudentNumber;
            if (dto.Email is not null) student.Email = dto.Email;

            if (dto.SectionId.HasValue)
            {
                var section = await _sectionRepo.GetByIdAsync(dto.SectionId.Value);
                if (section is null)
                    return BadRequest($"Section with ID {dto.SectionId} does not exist.");

                student.SectionId = dto.SectionId.Value;
            }

            await _studentRepo.UpdateAsync(student);

            var updated = await _studentRepo.GetByIdAsync(id);
            return Ok(MapToDTO(updated!));
        }

        /// <summary>Deletes a student and cascades to their attendance records.</summary>
        /// <param name="id">The student ID.</param>
        /// <response code="204">Deleted successfully.</response>
        /// <response code="404">No student with that ID.</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _studentRepo.DeleteAsync(id);
            if (!deleted) return NotFound($"Student with ID {id} not found.");
            return NoContent();
        }

        private static StudentResponseDTO MapToDTO(Student s) => new()
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            StudentNumber = s.StudentNumber,
            Email = s.Email,
            SectionId = s.SectionId,
            SectionName = s.Section?.Name ?? string.Empty,
            TotalPresent = s.Attendances.Count(a => a.Status == "Present"),
            TotalAbsent = s.Attendances.Count(a => a.Status == "Absent"),
            TotalLate = s.Attendances.Count(a => a.Status == "Late"),
            TotalExcused = s.Attendances.Count(a => a.Status == "Excused"),
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        };
    }
}