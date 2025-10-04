using MVC_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVC_Project.Interfaces
{
    public interface ICourseStudentsService
    {
        Task<IEnumerable<CourseStudents>> GetAllAsync();
        Task<CourseStudents> GetByIdAsync(int id);
        Task AddAsync(CourseStudents courseStudents);
        Task UpdateAsync(CourseStudents courseStudents);
        Task DeleteAsync(int id);
    }
}