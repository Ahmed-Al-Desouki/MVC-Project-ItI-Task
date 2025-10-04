using Microsoft.EntityFrameworkCore;

namespace MVC_Project.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<CourseStudents> CourseStudents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Setting precision and scale for decimal properties
            modelBuilder.Entity<Course>()
                .Property(c => c.Degree)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Course>()
                .Property(c => c.MinimumDegree)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<CourseStudents>()
                .Property(cs => cs.Degree)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Instructor>()
                .Property(i => i.Salary)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Student>()
                .Property(s => s.Grade)
                .HasColumnType("decimal(18,2)");
        }
    }
}