using Microsoft.EntityFrameworkCore;
using MVC_Project.Interfaces;
using MVC_Project.Models;

namespace MVC_Project.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly Context _context;
        private readonly ILogger<InstructorService> _logger;

        public InstructorService(Context context, ILogger<InstructorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Instructor>> GetAllAsync()
        {
            return await _context.Instructors
                .Include(i => i.Department)
                .Include(i => i.Course)
                .ToListAsync();
        }

        public async Task<Instructor> GetByIdAsync(int id)
        {
            return await _context.Instructors
                .Include(i => i.Department)
                .Include(i => i.Course)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddAsync(Instructor instructor)
        {
            _context.Instructors.Add(instructor);
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("Added instructor with ID {Id}, saved {Changes} changes", instructor.Id, changes);
            if (changes == 0)
            {
                _logger.LogWarning("No changes saved when adding instructor with ID {Id}", instructor.Id);
                throw new Exception("No changes were saved to the database.");
            }
        }

        public async Task UpdateAsync(Instructor instructor)
        {
            _context.Instructors.Update(instructor);
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("Updated instructor with ID {Id}, saved {Changes} changes", instructor.Id, changes);
            if (changes == 0)
            {
                _logger.LogWarning("No changes saved when updating instructor with ID {Id}", instructor.Id);
                throw new Exception("No changes were saved to the database.");
            }
        }

        public async Task DeleteAsync(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor != null)
            {
                _context.Instructors.Remove(instructor);
                var changes = await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted instructor with ID {Id}, saved {Changes} changes", id, changes);
                if (changes == 0)
                {
                    _logger.LogWarning("No changes saved when deleting instructor with ID {Id}", id);
                    throw new Exception("No changes were saved to the database.");
                }
            }
        }
    }
}