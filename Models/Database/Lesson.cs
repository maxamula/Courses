using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models.Database
{
    [Table("lessons")]
    public partial class Lesson
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public int Duration { get; set; }

        [Required]
        public int Sequence { get; set; }

        [Required]
        public int CourseID { get; set; }

        public ICollection<Appointment> Appointments { get; set; }

        public Course Course { get; set; }

    }
}