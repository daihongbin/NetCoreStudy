using Microsoft.EntityFrameworkCore;
using System;

namespace PostgresSample
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public Gender Gender { get; set; }

        public int Grade { get; set; }
    }

    public enum Gender
    {
        女,
        男
    }
}
