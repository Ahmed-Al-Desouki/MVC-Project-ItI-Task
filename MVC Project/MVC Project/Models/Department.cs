using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Manager Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Manager Name must be between 2 and 100 characters.")]
        public string ManagerName { get; set; } = default!;
    }
}