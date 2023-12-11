using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models.Database
{
    [Table("studentsgroups")]
    public partial class Studentsgroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string GroupName { get; set; }

        [Required]
        public int StudentID { get; set; }

        public Group Group { get; set; }

        public Student Student { get; set; }

    }
}