using AMS.Data;
using AMS.DTOs;
using AMS.Helpers;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace AMS.Services
{
    /// <summary>
    /// Handles user registration and login with JWT generation.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;   // PostgreSQL via EF Core
        private readonly JwtHelper _jwtHelper;

        public AuthService(ApplicationDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        // ── Register ───────────────────────────────────────────────────────
        public async Task<UserResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            // Check for duplicate email before inserting
            var exists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (exists)
                throw new InvalidOperationException($"Email '{dto.Email}' is already registered.");

            // Map DTO → model, hash the plain-text password
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = PasswordHelper.HashPassword(dto.Password), // never store plain text
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // commit to PostgreSQL

            return MapToDto(user);
        }

        // ── Login ──────────────────────────────────────────────────────────
        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO dto)
        {
            // Look up user by email (case-insensitive)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            // Return null if user not found or password is wrong
            if (user is null || !PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                return null;

            // Generate a signed JWT for the authenticated user
            return new AuthResponseDTO
            {
                Token = _jwtHelper.GenerateToken(user),
                Role = user.Role,
                FullName = $"{user.FirstName} {user.LastName}",
                UserId = user.Id,
                ExpiresAt = _jwtHelper.GetExpiry()
            };
        }

        // ── Helper ─────────────────────────────────────────────────────────
        private static UserResponseDTO MapToDto(User u) => new()
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
            // PasswordHash intentionally excluded from response
        };
    }
}