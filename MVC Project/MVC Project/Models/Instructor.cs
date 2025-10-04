using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Models
{
    public class Instructor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters.")]
        public string Address { get; set; } = default!;

        [Required(ErrorMessage = "Salary is required.")]
        [Range(1000, 1000000, ErrorMessage = "Salary must be between 1000 and 1,000,000.")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        public string Image { get; set; } = default!;

        [Required(ErrorMessage = "Department ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Department ID.")]
        public int DepartmentId { get; set; }
        // Removed [Required] from navigation property
        public Department? Department { get; set; }

        [Required(ErrorMessage = "Course ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Course ID.")]
        public int CourseId { get; set; }
        // Removed [Required] from navigation property
        public Course? Course { get; set; }
    }
}