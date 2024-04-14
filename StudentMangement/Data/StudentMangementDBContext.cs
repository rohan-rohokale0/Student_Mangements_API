
using Microsoft.EntityFrameworkCore;
using StudentMangement.Model;

namespace StudentMangement.Data
{
    public class StudentMangementDBContext : DbContext
    {
        public StudentMangementDBContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Student_Model>Student { get; set; }
        public DbSet<Category_Model> Category { get; set; }
        public DbSet<Course_Model> Course { get; set; }
        public DbSet<CourseRequestModel> CourseRequest { get; set; }
        public DbSet<User_Model> Users { get; set; }

    }
}
