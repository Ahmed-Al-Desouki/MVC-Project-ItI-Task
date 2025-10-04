using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Degree is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Degree must be a non-negative value.")]
        public decimal Degree { get; set; }

        [Required(ErrorMessage = "Minimum Degree is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum Degree must be a non-negative value.")]
        public decimal MinimumDegree { get; set; }

        [Required(ErrorMessage = "Hours is required.")]
        [Range(1, 1000, ErrorMessage = "Hours must be between 1 and 1000.")]
        public int Hours { get; set; }

        [Required(ErrorMessage = "Department ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Department ID.")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = default!;
    }
}