namespace MVC_Project.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal Degree { get; set; }
        public decimal MinimumDegree { get; set; }
        public int Hours { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = default!;
    }
}
