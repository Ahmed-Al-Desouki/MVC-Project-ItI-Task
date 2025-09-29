namespace MVC_Project.Models
{
    public class CourseStudents
    {
        public int Id { get; set; }
        public decimal Degree { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; } = default!;
        public int StudentId { get; set; }
        public Student Student { get; set; } = default!;
    }
}
