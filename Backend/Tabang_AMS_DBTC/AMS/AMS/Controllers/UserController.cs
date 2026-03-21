using AMS.DTOs;
using AMS.Helpers;
using AMS.Interfaces;
using AMS.Models;
using AMS.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    /// <summary>
    /// Manages user accounts including teachers and admins.
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

        /// <summary>Returns all users. Password hash is never included in the response.</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetAll()
        {
            var users = await _userRepo.GetAllAsync();
            return Ok(users.Select(MapToDTO));
        }

        /// <summary>Gets a single user by ID.</summary>
        /// <param name="id">The user ID.</param>
        /// <response code="200">User found.</response>
        /// <response code="404">No user with that ID.</response>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserResponseDTO>> GetById(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user is null) return NotFound($"User with ID {id} not found.");
            return Ok(MapToDTO(user));
        }

        /// <summary>Looks up a user by their email address.</summary>
        /// <param name="email">The user's email address.</param>
        /// <response code="200">User found.</response>
        /// <response code="404">No user with that email.</response>
        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserResponseDTO>> GetByEmail(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user is null) return NotFound($"No user found with email '{email}'.");
            return Ok(MapToDTO(user));
        }

        /// <summary>Returns all users with a given role.</summary>
        /// <param name="role">The role to filter by — either <c>Teacher</c> or <c>Admin</c>.</param>
        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetByRole(string role)
        {
            var users = await _userRepo.GetByRoleAsync(role);
            return Ok(users.Select(MapToDTO));
        }

        /// <summary>Creates a new user account. The password is hashed before storage.</summary>
        /// <response code="201">User created successfully.</response>
        /// <response code="409">A user with that email already exists.</response>
        [HttpPost]
        public async Task<ActionResult<UserResponseDTO>> Create(
            [FromBody] CreateUserDTO dto)
        {
            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing is not null)
                return Conflict($"A user with email '{dto.Email}' already exists.");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                Role = dto.Role
            };

            var created = await _userRepo.CreateAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDTO(created));
        }

        /// <summary>Partially or fully updates a user account.</summary>
        /// <remarks>Password is only re-hashed if a new value is provided in the request body.</remarks>
        /// <param name="id">The user ID.</param>
        /// <response code="200">User updated successfully.</response>
        /// <response code="404">No user with that ID.</response>
        /// <response code="409">The new email is already in use by another user.</response>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserResponseDTO>> Update(
            int id, [FromBody] UpdateUserDTO dto)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user is null) return NotFound($"User with ID {id} not found.");

            if (dto.FirstName is not null) user.FirstName = dto.FirstName;
            if (dto.LastName is not null) user.LastName = dto.LastName;

            if (dto.Email is not null)
            {
                var emailOwner = await _userRepo.GetByEmailAsync(dto.Email);
                if (emailOwner is not null && emailOwner.Id != id)
                    return Conflict($"Email '{dto.Email}' is already in use.");

                user.Email = dto.Email;
            }

            if (dto.Password is not null)
                user.PasswordHash = PasswordHelper.HashPassword(dto.Password);

            if (dto.Role is not null) user.Role = dto.Role;

            var updated = await _userRepo.UpdateAsync(user);
            return Ok(MapToDTO(updated));
        }

        /// <summary>Deletes a user account by ID.</summary>
        /// <param name="id">The user ID.</param>
        /// <response code="204">Deleted successfully.</response>
        /// <response code="404">No user with that ID.</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userRepo.DeleteAsync(id);
            if (!deleted) return NotFound($"User with ID {id} not found.");
            return NoContent();
        }

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