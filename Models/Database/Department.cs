using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models.Database
{
    [Table("departments")]
    public partial class Department
    {
        [Key]
        [Required]
        public string DepartmentName { get; set; }

        public ICollection<Course> Courses { get; set; }

    }
}