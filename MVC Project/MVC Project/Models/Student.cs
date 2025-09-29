namespace MVC_Project.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public decimal Grade { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = default!;
    }
}
