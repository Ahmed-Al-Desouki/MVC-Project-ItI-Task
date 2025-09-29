namespace MVC_Project.Models
{
    public class Context : DbContext
    {
        public DbSet<Course> Courses { set; get;}
        public DbSet<Student> Students { set; get; }
        public DbSet<Department> Departments { set; get; }
        public DbSet<Instractor> Instractors { set; get; }
        public DbSet<CourseStudents> CourseStudents { set; get; }

        public Context(DbContextOptions<Context> options) : base(options){}

    }
}
