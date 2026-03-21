using AMS.Data;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.EntityFrameworkCore;

namespace AMS.Repositories
{
    // Concrete implementation of ISectionRepository using EF Core + PostgreSQL
    public class SectionRepository : ISectionRepository
    {
        private readonly ApplicationDbContext _context;

        public SectionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── Retrieve all sections (including their teacher) ────────────
        public async Task<IEnumerable<Section>> GetAllAsync()
        {
            // Include the User (teacher) so callers can display the teacher name
            return await _context.Sections
                .Include(s => s.User)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        // ── Retrieve a section by PK (with teacher info) ───────────────
        public async Task<Section?> GetByIdAsync(int id)
        {
            return await _context.Sections
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        // ── Retrieve all sections assigned to a specific teacher ───────
        public async Task<IEnumerable<Section>> GetByUserIdAsync(int userId)
        {
            return await _context.Sections
                .Include(s => s.User)
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        // ── Retrieve a section with its enrolled students ──────────────
        public async Task<Section?> GetWithStudentsAsync(int sectionId)
        {
            // Eagerly load students to avoid lazy-loading N+1 queries
            return await _context.Sections
                .Include(s => s.User)
                .Include(s => s.Students)
                .FirstOrDefaultAsync(s => s.Id == sectionId);
        }

        // ── Create a new section ───────────────────────────────────────
        public async Task<Section> CreateAsync(Section section)
        {
            section.CreatedAt = DateTime.UtcNow;
            section.UpdatedAt = DateTime.UtcNow;

            _context.Sections.Add(section);
            await _context.SaveChangesAsync();

            return section;
        }

        // ── Update an existing section ─────────────────────────────────
        public async Task<Section> UpdateAsync(Section section)
        {
            section.UpdatedAt = DateTime.UtcNow;

            _context.Sections.Update(section);
            await _context.SaveChangesAsync();

            return section;
        }

        // ── Delete a section by ID ─────────────────────────────────────
        public async Task<bool> DeleteAsync(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section is null) return false;

            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}