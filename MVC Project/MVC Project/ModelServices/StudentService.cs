using MVC_Project.Interfaces;
using MVC_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MVC_Project.Services
{
    public class StudentService : IStudentService
    {
        private readonly Context _context;
        private readonly ILogger<StudentService> _logger;

        public StudentService(Context context, ILogger<StudentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students
                .Include(s => s.Department)
                .ToListAsync();
        }

        public async Task<Student> GetByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(Student student)
        {
            _context.Students.Add(student);
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("Added student with ID {Id}, saved {Changes} changes", student.Id, changes);
            if (changes == 0)
            {
                _logger.LogWarning("No changes saved when adding student with ID {Id}", student.Id);
                throw new Exception("No changes were saved to the database.");
            }
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("Updated student with ID {Id}, saved {Changes} changes", student.Id, changes);
            if (changes == 0)
            {
                _logger.LogWarning("No changes saved when updating student with ID {Id}", student.Id);
                throw new Exception("No changes were saved to the database.");
            }
        }

        public async Task DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                var changes = await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted student with ID {Id}, saved {Changes} changes", id, changes);
                if (changes == 0)
                {
                    _logger.LogWarning("No changes saved when deleting student with ID {Id}", id);
                    throw new Exception("No changes were saved to the database.");
                }
            }
        }
    }
}