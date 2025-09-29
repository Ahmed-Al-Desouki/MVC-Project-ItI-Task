namespace MVC_Project.Models
{
    public class Instractor
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public decimal Salary { get; set; }
        public string Image { get; set; } = default!;
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = default!;
        public int CourseId { get; set; }
        public Course Course { get; set; } = default!;
    }
}
