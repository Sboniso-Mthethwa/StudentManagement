using System.Data.Entity;

public class StudentDbContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Mark> Marks { get; set; }
    public DbSet<Stream> Streams { get; set; }

    public StudentDbContext() : base("StudentDbContext")
    {
    }
}
