using MVC_Project.Interfaces;
using MVC_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MVC_Project.Services
{
    public class CourseStudentsService : ICourseStudentsService
    {
        private readonly Context _context;
        private readonly ILogger<CourseStudentsService> _logger;

        public CourseStudentsService(Context context, ILogger<CourseStudentsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CourseStudents>> GetAllAsync()
        {
            return await _context.CourseStudents
                .Include(cs => cs.Course)
                .Include(cs => cs.Student)
                .ToListAsync();
        }

        public async Task<CourseStudents> GetByIdAsync(int id)
        {
            return await _context.CourseStudents
                .Include(cs => cs.Course)
                .Include(cs => cs.Student)
                .FirstOrDefaultAsync(cs => cs.Id == id);
        }

        public async Task AddAsync(CourseStudents courseStudents)
        {
            _context.CourseStudents.Add(courseStudents);
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("Added courseStudents with ID {Id}, saved {Changes} changes", courseStudents.Id, changes);
            if (changes == 0)
            {
                _logger.LogWarning("No changes saved when adding courseStudents with ID {Id}", courseStudents.Id);
                throw new Exception("No changes were saved to the database.");
            }
        }

        public async Task UpdateAsync(CourseStudents courseStudents)
        {
            _context.CourseStudents.Update(courseStudents);
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("Updated courseStudents with ID {Id}, saved {Changes} changes", courseStudents.Id, changes);
            if (changes == 0)
            {
                _logger.LogWarning("No changes saved when updating courseStudents with ID {Id}", courseStudents.Id);
                throw new Exception("No changes were saved to the database.");
            }
        }

        public async Task DeleteAsync(int id)
        {
            var courseStudents = await _context.CourseStudents.FindAsync(id);
            if (courseStudents != null)
            {
                _context.CourseStudents.Remove(courseStudents);
                var changes = await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted courseStudents with ID {Id}, saved {Changes} changes", id, changes);
                if (changes == 0)
                {
                    _logger.LogWarning("No changes saved when deleting courseStudents with ID {Id}", id);
                    throw new Exception("No changes were saved to the database.");
                }
            }
        }
    }
}