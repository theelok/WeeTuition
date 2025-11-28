using System.ComponentModel.DataAnnotations;

namespace TimetableSystem.Models
{
    public class TimetableViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public List<WeekViewModel> Weeks { get; set; }
        public List<ScheduleStatus> StatusLegend { get; set; }
        public bool IsTeacher { get; set; }
        public int? CurrentStudentId { get; set; }
    }

    public class WeekViewModel
    {
        public List<DayViewModel> Days { get; set; }
    }

    public class DayViewModel
    {
        public DateTime Date { get; set; }
        public int DayOfMonth { get; set; }
        public string DayOfWeek { get; set; }
        public bool IsCurrentMonth { get; set; }
        public List<TimetableEntryViewModel> Entries { get; set; }
    }

    public class TimetableEntryViewModel
    {
        public int EntryId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public string SubjectCode { get; set; }
        public string Subject { get; set; }
        public string StatusName { get; set; }
        public string ColorCode { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public string TimeSlot { get; set; }
    }

    public class EditEntryViewModel
    {
        public int EntryId { get; set; }
        public int StudentId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int StatusId { get; set; }
        public string Subject { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }

        public List<Student>? Students { get; set; }
        public List<ScheduleStatus>? Statuses { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
