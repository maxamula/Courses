using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models.Database
{
    [Table("appointments")]
    public partial class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int LessonId { get; set; }

        [Required]
        public string GroupName { get; set; }

        [Required]
        public DateTime LessonStart { get; set; }
        [Required]
        public DateTime LessonEnd { get; set; }

        public Group Group { get; set; }

        public Lesson Lesson { get; set; }

    }
}