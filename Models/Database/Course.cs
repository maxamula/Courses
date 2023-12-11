using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models.Database
{
    [Table("courses")]
    public partial class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public float PaymentPerHour { get; set; }

        [Required]
        public string CourseType { get; set; }

        public byte DaysOfWeek { get; set; }

        public bool Archived { get; set; }

        [Required]
        public int TeacherID { get; set; }

        [Required]
        public string DepartmentName { get; set; }

        public byte[] Picture { get; set; }

        public Department Department { get; set; }

        public Teacher Teacher { get; set; }

        public ICollection<Group> Groups { get; set; }

        public ICollection<Lesson> Lessons { get; set; }

    }
}