using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Models
{
    public class CourseStudents
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Degree is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Degree must be a non-negative value.")]
        public decimal Degree { get; set; }

        [Required(ErrorMessage = "Course ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Course ID.")]
        public int CourseId { get; set; }

        public Course? Course { get; set; }

        [Required(ErrorMessage = "Student ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Student ID.")]
        public int StudentId { get; set; }

        public Student? Student { get; set; }
    }
}