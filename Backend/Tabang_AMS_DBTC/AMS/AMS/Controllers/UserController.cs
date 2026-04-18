using AMS.DTOs;
using AMS.Helpers;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    /// <summary>
    /// API for managing system users such as Teachers and Admins.
    /// Supports pagination, searching, and CRUD operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        /// <summary>
        /// Gets a paginated list of users.
        /// </summary>
        /// <param name="email">Search by email</param>
        /// <param name="name">Search by first or last name</param>
        /// <param name="pageNumber">Page number (default = 1)</param>
        /// <param name="pageSize">Number of records per page (default = 10)</param>
        /// <returns>Paginated list of users</returns>
        /// <response code="200">Users retrieved successfully</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetUsers(
            string? email = null,
            string? role = null,
            string? name = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var users = await _userRepo.GetAllAsync();

            // Filtering
            if (!string.IsNullOrWhiteSpace(email))
                users = users.Where(u => u.Email.Contains(email, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(name))
                users = users.Where(u =>
                    u.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                    u.LastName.Contains(name, StringComparison.OrdinalIgnoreCase));

            // Pagination
            var pagedUsers = users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDTO);

            return Ok(pagedUsers);
        }

        /// <summary>
        /// Creates a new user account.
        /// </summary>
        /// <param name="dto">User creation data</param>
        /// <response code="201">User created successfully</response>
        /// <response code="409">Email already exists</response>
        /// <response code="400">Invalid input</response>
        [HttpPost]
        public async Task<ActionResult<UserResponseDTO>> CreateUser([FromBody] CreateUserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                return Conflict($"User with email '{dto.Email}' already exists.");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                Role = dto.Role
            };

            var created = await _userRepo.CreateAsync(user);

            return Created("", MapToDTO(created));
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="dto">Updated user data</param>
        /// <response code="200">User updated</response>
        /// <response code="404">User not found</response>
        /// <response code="409">Email already used</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDTO>> UpdateUser(int id, [FromBody] UpdateUserDTO dto)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");

            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrWhiteSpace(dto.LastName))
                user.LastName = dto.LastName;

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var emailOwner = await _userRepo.GetByEmailAsync(dto.Email);
                if (emailOwner != null && emailOwner.Id != id)
                    return Conflict("Email already used by another user.");

                user.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = PasswordHelper.HashPassword(dto.Password);

            if (!string.IsNullOrWhiteSpace(dto.Role))
                user.Role = dto.Role;

            var updated = await _userRepo.UpdateAsync(user);

            return Ok(MapToDTO(updated));
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="204">User deleted</response>
        /// <response code="404">User not found</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userRepo.DeleteAsync(id);

            if (!deleted)
                return NotFound($"User with ID {id} not found.");

            return NoContent();
        }

        /// <summary>
        /// Converts User model to response DTO.
        /// </summary>
        private static UserResponseDTO MapToDTO(User u) => new()
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        };
    }
}