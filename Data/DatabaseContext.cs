using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Courses.Models.Database;

namespace Courses.Data
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Courses.Models.Database.Account>()
              .HasOne(i => i.Student)
              .WithMany(i => i.Accounts)
              .HasForeignKey(i => i.StudentId)
              .HasPrincipalKey(i => i.ID);

            builder.Entity<Courses.Models.Database.Appointment>()
              .HasOne(i => i.Group)
              .WithMany(i => i.Appointments)
              .HasForeignKey(i => i.GroupName)
              .HasPrincipalKey(i => i.Name);

            builder.Entity<Courses.Models.Database.Appointment>()
              .HasOne(i => i.Lesson)
              .WithMany(i => i.Appointments)
              .HasForeignKey(i => i.LessonId)
              .HasPrincipalKey(i => i.ID);

            builder.Entity<Courses.Models.Database.Course>()
              .HasOne(i => i.Department)
              .WithMany(i => i.Courses)
              .HasForeignKey(i => i.DepartmentName)
              .HasPrincipalKey(i => i.DepartmentName);

            builder.Entity<Courses.Models.Database.Course>()
              .HasOne(i => i.Teacher)
              .WithMany(i => i.Courses)
              .HasForeignKey(i => i.TeacherID)
              .HasPrincipalKey(i => i.ID);

            builder.Entity<Courses.Models.Database.Group>()
              .HasOne(i => i.Course)
              .WithMany(i => i.Groups)
              .HasForeignKey(i => i.CourseID)
              .HasPrincipalKey(i => i.ID);

            builder.Entity<Courses.Models.Database.Lesson>()
              .HasOne(i => i.Course)
              .WithMany(i => i.Lessons)
              .HasForeignKey(i => i.CourseID)
              .HasPrincipalKey(i => i.ID);

            builder.Entity<Courses.Models.Database.Studentsgroup>()
              .HasOne(i => i.Group)
              .WithMany(i => i.Studentsgroups)
              .HasForeignKey(i => i.GroupName)
              .HasPrincipalKey(i => i.Name);

            builder.Entity<Courses.Models.Database.Studentsgroup>()
              .HasOne(i => i.Student)
              .WithMany(i => i.Studentsgroups)
              .HasForeignKey(i => i.StudentID)
              .HasPrincipalKey(i => i.ID);

            builder.Entity<Courses.Models.Database.Course>()
              .Property(p => p.DaysOfWeek)
              .HasDefaultValueSql(@"'1'");

            builder.Entity<Courses.Models.Database.Course>()
              .Property(p => p.Archived)
              .HasDefaultValueSql(@"false");

            builder.Entity<Courses.Models.Database.Lesson>()
              .Property(p => p.Duration)
              .HasDefaultValueSql(@"'60'");

            builder.Entity<Courses.Models.Database.Teacher>()
              .Property(p => p.Archived)
              .HasDefaultValueSql(@"false");

            builder.Entity<Courses.Models.Database.Appointment>()
              .Property(p => p.LessonStart)
              .HasColumnType("datetime");

            builder.Entity<Courses.Models.Database.Appointment>()
              .Property(p => p.LessonEnd)
              .HasColumnType("datetime");
            this.OnModelBuilding(builder);
        }

        public DbSet<Courses.Models.Database.Account> Accounts { get; set; }

        public DbSet<Courses.Models.Database.Appointment> Appointments { get; set; }

        public DbSet<Courses.Models.Database.Course> Courses { get; set; }

        public DbSet<Courses.Models.Database.Department> Departments { get; set; }

        public DbSet<Courses.Models.Database.Group> Groups { get; set; }

        public DbSet<Courses.Models.Database.Lesson> Lessons { get; set; }

        public DbSet<Courses.Models.Database.Student> Students { get; set; }

        public DbSet<Courses.Models.Database.Studentsgroup> Studentsgroups { get; set; }

        public DbSet<Courses.Models.Database.Teacher> Teachers { get; set; }

    }
}