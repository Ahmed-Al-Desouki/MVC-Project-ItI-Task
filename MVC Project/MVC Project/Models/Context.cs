namespace MVC_Project.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<Course> Courses { set; get;}
        public DbSet<Student> Students { set; get; }
        public DbSet<Department> Departments { set; get; }
        public DbSet<Instractor> Instractors { set; get; }
        public DbSet<CourseStudents> CourseStudents { set; get; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تحديد الدقة والمقياس للخصائص من نوع decimal
            modelBuilder.Entity<Course>()
                .Property(c => c.Degree)
                .HasColumnType("decimal(18,2)"); // 18 رقم إجمالي، 2 بعد الفاصلة
            modelBuilder.Entity<Course>()
                .Property(c => c.MinimumDegree)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<CourseStudents>()
                .Property(cs => cs.Degree)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Instractor>()
                .Property(i => i.Salary)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Student>()
                .Property(s => s.Grade)
                .HasColumnType("decimal(18,2)");
        }
    }
}
