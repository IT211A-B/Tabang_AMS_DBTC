using AMS.Data;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.EntityFrameworkCore;

namespace AMS.Repositories
{
    // Concrete implementation of IUserRepository using EF Core + PostgreSQL
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        // Inject the DbContext via constructor injection
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── Retrieve all users ─────────────────────────────────────────
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            // Order alphabetically for a consistent default sort
            return await _context.Users
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }

        // ── Retrieve a single user by primary key ──────────────────────
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // ── Find a user by their email address ─────────────────────────
        public async Task<User?> GetByEmailAsync(string email)
        {
            // Use case-insensitive comparison to handle different casing
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        // ── Get all users with a specific role ─────────────────────────
        public async Task<IEnumerable<User>> GetByRoleAsync(string role)
        {
            return await _context.Users
                .Where(u => u.Role == role)
                .OrderBy(u => u.LastName)
                .ToListAsync();
        }

        // ── Insert a new user ──────────────────────────────────────────
        public async Task<User> CreateAsync(User user)
        {
            // Set timestamps on creation
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // commit to PostgreSQL

            return user;
        }

        // ── Update an existing user ────────────────────────────────────
        public async Task<User> UpdateAsync(User user)
        {
            // Always update the UpdatedAt timestamp on modification
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // ── Delete a user by ID ────────────────────────────────────────
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return false; // record not found

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true; // successfully deleted
        }
    }
}