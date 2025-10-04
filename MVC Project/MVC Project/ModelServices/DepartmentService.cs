using MVC_Project.Interfaces;
using MVC_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MVC_Project.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly Context _context;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(Context context, ILogger<DepartmentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<Department> GetByIdAsync(int id)
        {
            return await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task AddAsync(Department department)
        {
            _context.Departments.Add(department);
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("Added department with ID {Id}, saved {Changes} changes", department.Id, changes);
            if (changes == 0)
            {
                _logger.LogWarning("No changes saved when adding department with ID {Id}", department.Id);
                throw new Exception("No changes were saved to the database.");
            }
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("Updated department with ID {Id}, saved {Changes} changes", department.Id, changes);
            if (changes == 0)
            {
                _logger.LogWarning("No changes saved when updating department with ID {Id}", department.Id);
                throw new Exception("No changes were saved to the database.");
            }
        }

        public async Task DeleteAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                var changes = await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted department with ID {Id}, saved {Changes} changes", id, changes);
                if (changes == 0)
                {
                    _logger.LogWarning("No changes saved when deleting department with ID {Id}", id);
                    throw new Exception("No changes were saved to the database.");
                }
            }
        }
    }
}