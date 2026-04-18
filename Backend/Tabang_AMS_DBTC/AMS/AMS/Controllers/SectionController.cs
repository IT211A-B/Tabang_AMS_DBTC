using AMS.DTOs;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    /// <summary>
    /// API for managing sections and assigned teachers.
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

        /// <summary>
        /// Get paginated sections with optional search filters.
        /// </summary>
        /// <param name="name">Filter by section name</param>
        /// <param name="teacherId">Filter by teacher ID</param>
        /// <param name="schoolYear">Filter by school year</param>
        /// <param name="pageNumber">Page number (default = 1)</param>
        /// <param name="pageSize">Records per page (default = 10)</param>
        /// <response code="200">Returns paginated section list</response>
        [HttpGet]
        public async Task<ActionResult<object>> Get(
            [FromQuery] string? name = null,
            [FromQuery] int? teacherId = null,
            [FromQuery] string? schoolYear = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("PageNumber and PageSize must be greater than 0.");

            var sections = await _sectionRepo.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(name))
                sections = sections.Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

            if (teacherId.HasValue)
                sections = sections.Where(s => s.UserId == teacherId);

            if (!string.IsNullOrWhiteSpace(schoolYear))
                sections = sections.Where(s => s.SchoolYear == schoolYear);

            var total = sections.Count();

            var data = sections
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDTO);

            return Ok(new
            {
                pageNumber,
                pageSize,
                totalRecords = total,
                totalPages = (int)Math.Ceiling(total / (double)pageSize),
                data
            });
        }

        /// <summary>
        /// Create a new section.
        /// </summary>
        /// <response code="201">Section created</response>
        /// <response code="400">Invalid teacher ID</response>
        [HttpPost]
        public async Task<ActionResult<SectionResponseDTO>> Post([FromBody] CreateSectionDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Section name is required.");

            var teacher = await _userRepo.GetByIdAsync(dto.UserId);
            if (teacher is null)
                return BadRequest($"Teacher with ID {dto.UserId} not found.");

            var section = new Section
            {
                Name = dto.Name,
                SchoolYear = dto.SchoolYear,
                Semester = dto.Semester,
                UserId = dto.UserId
            };

            var created = await _sectionRepo.CreateAsync(section);
            return Ok(MapToDTO(created));
        }

        /// <summary>
        /// Update a section.
        /// </summary>
        /// <param name="id">Section ID</param>
        /// <response code="200">Section updated</response>
        /// <response code="404">Section not found</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<SectionResponseDTO>> Put(int id, [FromBody] UpdateSectionDTO dto)
        {
            var section = await _sectionRepo.GetByIdAsync(id);
            if (section is null) return NotFound($"Section {id} not found.");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                section.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.SchoolYear))
                section.SchoolYear = dto.SchoolYear;

            if (!string.IsNullOrWhiteSpace(dto.Semester))
                section.Semester = dto.Semester;

            if (dto.UserId.HasValue)
            {
                var teacher = await _userRepo.GetByIdAsync(dto.UserId.Value);
                if (teacher is null)
                    return BadRequest($"Teacher {dto.UserId} not found.");

                section.UserId = dto.UserId.Value;
            }

            await _sectionRepo.UpdateAsync(section);
            return Ok(MapToDTO(section));
        }

        /// <summary>
        /// Delete a section.
        /// </summary>
        /// <param name="id">Section ID</param>
        /// <response code="204">Deleted successfully</response>
        /// <response code="404">Section not found</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _sectionRepo.DeleteAsync(id);
            if (!deleted) return NotFound($"Section {id} not found.");

            return NoContent();
        }

        /// <summary>
        /// Maps Section model to response DTO.
        /// </summary>
        private static SectionResponseDTO MapToDTO(Section s) => new()
        {
            Id = s.Id,
            Name = s.Name,
            SchoolYear = s.SchoolYear,
            Semester = s.Semester,
            UserId = s.UserId,
            TeacherName = s.User != null ? $"{s.User.FirstName} {s.User.LastName}" : "",
            StudentCount = s.Students?.Count ?? 0,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        };
    }
}