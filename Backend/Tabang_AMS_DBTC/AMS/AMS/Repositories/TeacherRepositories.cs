using AMS.Interfaces;
using AMS.Models;

namespace AMS.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly List<Teacher> _teachers = new();

        public Task<IEnumerable<Teacher>> GetAllAsync() =>
            Task.FromResult(_teachers.AsEnumerable());

        public Task<Teacher?> GetByIdAsync(int id) =>
            Task.FromResult(_teachers.FirstOrDefault(t => t.Id == id));

        public Task<Teacher?> GetByEmailAsync(string email) =>
            Task.FromResult(_teachers.FirstOrDefault(t =>
                t.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));

        public Task<IEnumerable<Teacher>> SearchByNameAsync(string name) =>
            Task.FromResult(_teachers.Where(t =>
                t.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                t.LastName.Contains(name, StringComparison.OrdinalIgnoreCase)));

        public Task<Teacher> CreateAsync(Teacher teacher)
        {
            teacher.Id = _teachers.Count + 1;
            teacher.CreatedAt = DateTime.UtcNow;
            teacher.UpdatedAt = DateTime.UtcNow;

            _teachers.Add(teacher);
            return Task.FromResult(teacher);
        }

        public Task<Teacher> UpdateAsync(Teacher teacher)
        {
            teacher.UpdatedAt = DateTime.UtcNow;
            return Task.FromResult(teacher);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var teacher = _teachers.FirstOrDefault(t => t.Id == id);
            if (teacher == null) return Task.FromResult(false);

            _teachers.Remove(teacher);
            return Task.FromResult(true);
        }
    }
}