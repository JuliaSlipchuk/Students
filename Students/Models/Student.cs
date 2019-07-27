using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Students.Models
{
    [Table("Student")]
    public class Student
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public float AvgMark { get; set; }
        [Required]
        public int CourseNumb { get; set; }
        [Required]
        public byte[] Image { get; set; }
        public string Extension { get; set; } = "jpg";
        [Required]
        public string Email { get; set; }
        [Required]
        public string Courses { get; set; }
    }
}