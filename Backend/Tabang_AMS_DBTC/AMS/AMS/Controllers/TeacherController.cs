using AMS.DTOs;
using AMS.Models;
using AMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    /// <summary>
    /// Handles Teacher management (CRUD + search + pagination).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly TeacherService _service;

        public TeachersController(TeacherService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get teachers with pagination and search.
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Items per page</param>
        /// <param name="name">Search by name</param>
        /// <param name="email">Search by email</param>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherDTO>>> Get(
            int pageNumber = 1,
            int pageSize = 10,
            string? name = null,
            string? email = null)
        {
            var teachers = await _service.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(name))
                teachers = teachers.Where(t =>
                    t.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                    t.LastName.Contains(name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(email))
                teachers = teachers.Where(t =>
                    t.Email.Contains(email, StringComparison.OrdinalIgnoreCase));

            var result = teachers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(Map);

            return Ok(result);
        }

        /// <summary>Create new teacher</summary>
        [HttpPost]
        public async Task<ActionResult<TeacherDTO>> Post(CreateTeacherDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _service.GetByEmailAsync(dto.Email);
            if (existing != null)
                return Conflict("Email already exists.");

            var teacher = new Teacher
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Department = dto.Department
            };

            var created = await _service.CreateAsync(teacher);

            return Created("", Map(created));
        }

        /// <summary>Update teacher</summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TeacherDTO>> Put(int id, UpdateTeacherDTO dto)
        {
            var teacher = await _service.GetByIdAsync(id);
            if (teacher == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.FirstName)) teacher.FirstName = dto.FirstName;
            if (!string.IsNullOrWhiteSpace(dto.LastName)) teacher.LastName = dto.LastName;
            if (!string.IsNullOrWhiteSpace(dto.Email)) teacher.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Phone)) teacher.Phone = dto.Phone;
            if (!string.IsNullOrWhiteSpace(dto.Department)) teacher.Department = dto.Department;

            var updated = await _service.UpdateAsync(teacher);

            return Ok(Map(updated));
        }

        /// <summary>Delete teacher</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }

        private static TeacherDTO Map(Teacher t) => new()
        {
            Id = t.Id,
            FirstName = t.FirstName,
            LastName = t.LastName,
            Email = t.Email,
            Phone = t.Phone,
            Department = t.Department
        };
    }
}