﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BullDoghs.Models
{
    [Table("tbCourseSection")]
    public partial class tbCourseSection
    {
        public tbCourseSection()
        {
            tbRegistrations = new HashSet<tbRegistration>();
        }

        [Key]
        public int CourseSectionID_PK { get; set; }
        [Required]
        [StringLength(15)]
        public string Course_day { get; set; }
        [Required]
        [StringLength(30)]
        public string Course_time { get; set; }
        public int AttendentNumber { get; set; }
        public int ClassRoom { get; set; }
        [Required]
        [StringLength(20)]
        public string TeacherID_FK { get; set; }
        [Required]
        [StringLength(50)]
        public string CourseTitle_FK { get; set; }

        [ForeignKey("CourseTitle_FK")]
        [InverseProperty("tbCourseSections")]
        public virtual tbCourse CourseTitle_FKNavigation { get; set; }
        [ForeignKey("TeacherID_FK")]
        [InverseProperty("tbCourseSections")]
        public virtual tbTeacher TeacherID_FKNavigation { get; set; }
        [InverseProperty("CourseSectionID_FKNavigation")]
        public virtual ICollection<tbRegistration> tbRegistrations { get; set; }
    }
}