using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimetableSystem.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<ScheduleStatus> ScheduleStatuses { get; set; }
        public DbSet<TimetableEntry> TimetableEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        [StringLength(200)]
        public string StudentName { get; set; }

        [Required]
        [StringLength(50)]
        public string StudentCode { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ICollection<TimetableEntry> TimetableEntries { get; set; }
    }

    public class ScheduleStatus
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusName { get; set; }

        [Required]
        [StringLength(20)]
        public string ColorCode { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public virtual ICollection<TimetableEntry> TimetableEntries { get; set; }
    }

    public class TimetableEntry
    {
        [Key]
        public int EntryId { get; set; }

        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [Required]
        public DateTime ScheduleDate { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public virtual ScheduleStatus Status { get; set; }

        [StringLength(100)]
        public string? Subject { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; }
    }

}