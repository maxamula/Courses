using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models.Database
{
    [Table("accounts")]
    public partial class Account
    {
        [Key]
        [Required]
        public string Name { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public float Balance { get; set; }

        public int? StudentId { get; set; }

        public Student Student { get; set; }

    }
}