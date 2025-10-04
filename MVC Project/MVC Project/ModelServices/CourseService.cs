using MVC_Project.Interfaces;
using MVC_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MVC_Project.Services
{
    public class CourseService : ICourseService
    {
        private readonly Context _context;
        private readonly ILogger<CourseService> _logger;

        public CourseService(Context context, ILogger<CourseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(c => c.Department)
                .ToListAsync();
        }

        public async Task<Course> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Course course)
        {
            _context.Courses.Add(course);
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("Added course with ID {Id}, saved {Changes} changes", course.Id, changes);
            if (changes == 0)
            {
                _logger.LogWarning("No changes saved when adding course with ID {Id}", course.Id);
                throw new Exception("No changes were saved to the database.");
            }
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("Updated course with ID {Id}, saved {Changes} changes", course.Id, changes);
            if (changes == 0)
            {
                _logger.LogWarning("No changes saved when updating course with ID {Id}", course.Id);
                throw new Exception("No changes were saved to the database.");
            }
        }

        public async Task DeleteAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                var changes = await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted course with ID {Id}, saved {Changes} changes", id, changes);
                if (changes == 0)
                {
                    _logger.LogWarning("No changes saved when deleting course with ID {Id}", id);
                    throw new Exception("No changes were saved to the database.");
                }
            }
        }
    }
}