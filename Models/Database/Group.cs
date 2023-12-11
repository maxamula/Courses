using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models.Database
{
    [Table("groups")]
    public partial class Group
    {
        [Key]
        [Required]
        public string Name { get; set; }

        [Required]
        public int CourseID { get; set; }

        public ICollection<Appointment> Appointments { get; set; }

        public Course Course { get; set; }

        public ICollection<Studentsgroup> Studentsgroups { get; set; }

    }
}