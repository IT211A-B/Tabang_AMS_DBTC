using AMS.DTOs;
using AMS.Interfaces;
using AMS.Models;
using AMS.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    /// <summary>
    /// Manages sections and their assigned teachers.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SectionsController : ControllerBase
    {
        private readonly ISectionRepository _sectionRepo;
        private readonly IUserRepository _userRepo;

        public SectionsController(ISectionRepository sectionRepo, IUserRepository userRepo)
        {
            _sectionRepo = sectionRepo;
            _userRepo = userRepo;
        }

        /// <summary>Returns all sections with their assigned teacher name.</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SectionResponseDTO>>> GetAll()
        {
            var sections = await _sectionRepo.GetAllAsync();
            return Ok(sections.Select(MapToDTO));
        }

        /// <summary>Gets a single section by ID.</summary>
        /// <param name="id">The section ID.</param>
        /// <response code="200">Section found.</response>
        /// <response code="404">No section with that ID.</response>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SectionResponseDTO>> GetById(int id)
        {
            var section = await _sectionRepo.GetByIdAsync(id);
            if (section is null) return NotFound($"Section with ID {id} not found.");
            return Ok(MapToDTO(section));
        }

        /// <summary>Returns a section together with its full enrolled student list.</summary>
        /// <param name="id">The section ID.</param>
        /// <response code="200">Section with students found.</response>
        /// <response code="404">No section with that ID.</response>
        [HttpGet("{id:int}/students")]
        public async Task<ActionResult<SectionResponseDTO>> GetWithStudents(int id)
        {
            var section = await _sectionRepo.GetWithStudentsAsync(id);
            if (section is null) return NotFound($"Section with ID {id} not found.");
            return Ok(MapToDTO(section));
        }

        /// <summary>Returns all sections assigned to a specific teacher.</summary>
        /// <param name="userId">The teacher's user ID.</param>
        [HttpGet("teacher/{userId:int}")]
        public async Task<ActionResult<IEnumerable<SectionResponseDTO>>> GetByTeacher(int userId)
        {
            var sections = await _sectionRepo.GetByUserIdAsync(userId);
            return Ok(sections.Select(MapToDTO));
        }

        /// <summary>Creates a new section and assigns it to a teacher.</summary>
        /// <response code="201">Section created successfully.</response>
        /// <response code="400">The provided teacher user ID does not exist.</response>
        [HttpPost]
        public async Task<ActionResult<SectionResponseDTO>> Create(
            [FromBody] CreateSectionDTO dto)
        {
            var teacher = await _userRepo.GetByIdAsync(dto.UserId);
            if (teacher is null)
                return BadRequest($"User (teacher) with ID {dto.UserId} does not exist.");

            var section = new Section
            {
                Name = dto.Name,
                SchoolYear = dto.SchoolYear,
                Semester = dto.Semester,
                UserId = dto.UserId
            };

            var created = await _sectionRepo.CreateAsync(section);
            var withUser = await _sectionRepo.GetByIdAsync(created.Id);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDTO(withUser!));
        }

        /// <summary>Partially or fully updates a section.</summary>
        /// <param name="id">The section ID.</param>
        /// <response code="200">Section updated successfully.</response>
        /// <response code="400">The provided teacher user ID does not exist.</response>
        /// <response code="404">No section with that ID.</response>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<SectionResponseDTO>> Update(
            int id, [FromBody] UpdateSectionDTO dto)
        {
            var section = await _sectionRepo.GetByIdAsync(id);
            if (section is null) return NotFound($"Section with ID {id} not found.");

            if (dto.Name is not null) section.Name = dto.Name;
            if (dto.SchoolYear is not null) section.SchoolYear = dto.SchoolYear;
            if (dto.Semester is not null) section.Semester = dto.Semester;

            if (dto.UserId.HasValue)
            {
                var teacher = await _userRepo.GetByIdAsync(dto.UserId.Value);
                if (teacher is null)
                    return BadRequest($"User (teacher) with ID {dto.UserId} does not exist.");

                section.UserId = dto.UserId.Value;
            }

            await _sectionRepo.UpdateAsync(section);

            var updated = await _sectionRepo.GetByIdAsync(id);
            return Ok(MapToDTO(updated!));
        }

        /// <summary>Deletes a section by ID.</summary>
        /// <param name="id">The section ID.</param>
        /// <response code="204">Deleted successfully.</response>
        /// <response code="404">No section with that ID.</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _sectionRepo.DeleteAsync(id);
            if (!deleted) return NotFound($"Section with ID {id} not found.");
            return NoContent();
        }

        private static SectionResponseDTO MapToDTO(Section s) => new()
        {
            Id = s.Id,
            Name = s.Name,
            SchoolYear = s.SchoolYear,
            Semester = s.Semester,
            UserId = s.UserId,
            TeacherName = s.User is not null
                               ? $"{s.User.FirstName} {s.User.LastName}"
                               : string.Empty,
            StudentCount = s.Students?.Count ?? 0,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        };
    }
}