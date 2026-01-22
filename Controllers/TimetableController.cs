using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimetableSystem.Models;

namespace TimetableSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimetableController : ControllerBase
    {
        private readonly AppDbContext db;

        public TimetableController(AppDbContext context)
        {
            db = context;
        }

        // GET: api/Timetable
        [HttpGet]
        public IActionResult GetTimetable([FromQuery] int? year, [FromQuery] int? month)
        {
            bool isTeacher = HttpContext.Session.GetString("IsTeacher") == "true";
            int? studentId = HttpContext.Session.GetInt32("StudentId");

            if (!isTeacher && !studentId.HasValue)
                return Unauthorized(new { message = "Not logged in" });

            if (!year.HasValue || !month.HasValue)
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
            }

            var viewModel = BuildTimetableViewModel(year.Value, month.Value, isTeacher, studentId);
            return Ok(viewModel);
        }

        // GET: api/Timetable/{id}
        [HttpGet("{id}")]
        public IActionResult GetEntry(int id)
        {
            bool isTeacher = HttpContext.Session.GetString("IsTeacher") == "true";
            if (!isTeacher)
                return Unauthorized(new { message = "Only teachers can view entry details" });

            var entry = db.TimetableEntries
                .Include(t => t.Student)
                .Include(t => t.Status)
                .FirstOrDefault(t => t.EntryId == id);

            if (entry == null)
                return NotFound();

            // Get students and statuses without navigation properties
            var students = db.Students.AsNoTracking().ToList();
            var statuses = db.ScheduleStatuses
                .AsNoTracking()
                .Select(s => new ScheduleStatus
                {
                    StatusId = s.StatusId,
                    StatusName = s.StatusName,
                    ColorCode = s.ColorCode,
                    Description = s.Description
                })
                .ToList();
            
            // Clear navigation properties
            foreach (var status in statuses)
            {
                status.TimetableEntries = null;
            }

            var viewModel = new EditEntryViewModel
            {
                EntryId = entry.EntryId,
                StudentId = entry.StudentId,
                ScheduleDate = entry.ScheduleDate,
                StartTime = entry.StartTime.ToString(@"HH\:mm") ?? "09:00",
                EndTime = entry.EndTime.ToString(@"HH\:mm") ?? "10:00",
                StatusId = entry.StatusId,
                Subject = entry.Subject,
                Location = entry.Location,
                Notes = entry.Notes,
                Students = students,
                Statuses = statuses
            };

            return Ok(viewModel);
        }

        // GET: api/Timetable/students
        [HttpGet("students")]
        public IActionResult GetStudents()
        {
            var students = db.Students.ToList();
            return Ok(students);
        }

        // GET: api/Timetable/statuses
        [HttpGet("statuses")]
        public IActionResult GetStatuses()
        {
            var statuses = db.ScheduleStatuses
                .AsNoTracking()
                .Select(s => new ScheduleStatusDto
                {
                    StatusId = s.StatusId,
                    StatusName = s.StatusName,
                    ColorCode = s.ColorCode,
                    Description = s.Description
                })
                .ToList();
            return Ok(statuses);
        }

        // PUT: api/Timetable/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateEntry(int id, [FromBody] EditEntryViewModel model)
        {
            bool isTeacher = HttpContext.Session.GetString("IsTeacher") == "true";
            if (!isTeacher)
                return Unauthorized(new { message = "Only teachers can edit entries" });

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid model data", errors = ModelState });
            }

            // Validate time
            if (!string.IsNullOrEmpty(model.StartTime) && !string.IsNullOrEmpty(model.EndTime))
            {
                var startParts = model.StartTime.Split(':');
                var endParts = model.EndTime.Split(':');

                if (startParts.Length != 2 || endParts.Length != 2)
                {
                    return BadRequest(new { message = "Invalid time format. Use HH:mm" });
                }

                var startMinutes = int.Parse(startParts[0]) * 60 + int.Parse(startParts[1]);
                var endMinutes = int.Parse(endParts[0]) * 60 + int.Parse(endParts[1]);

                if (endMinutes <= startMinutes)
                {
                    return BadRequest(new { message = "End time must be later than start time" });
                }
            }

            try
            {
                var entry = db.TimetableEntries.FirstOrDefault(t => t.EntryId == id);
                if (entry == null)
                    return NotFound();

                entry.StudentId = model.StudentId;
                entry.ScheduleDate = DateTime.SpecifyKind(model.ScheduleDate.Date, DateTimeKind.Utc);

                var startTimeParts = model.StartTime.Split(':');
                entry.StartTime = new DateTime(1, 1, 1, int.Parse(startTimeParts[0]), int.Parse(startTimeParts[1]), 0, DateTimeKind.Utc);

                var endTimeParts = model.EndTime.Split(':');
                entry.EndTime = new DateTime(1, 1, 1, int.Parse(endTimeParts[0]), int.Parse(endTimeParts[1]), 0, DateTimeKind.Utc);

                entry.StatusId = model.StatusId;
                entry.Subject = model.Subject;
                entry.Location = model.Location;
                entry.Notes = model.Notes;

                db.SaveChanges();

                return Ok(new { success = true, message = "Entry updated successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error saving changes: " + ex.Message });
            }
        }

        // POST: api/Timetable
        [HttpPost]
        public IActionResult CreateEntry([FromBody] EditEntryViewModel model)
        {
            bool isTeacher = HttpContext.Session.GetString("IsTeacher") == "true";
            if (!isTeacher)
                return Unauthorized(new { message = "Only teachers can create entries" });

            // Validate time
            if (!string.IsNullOrEmpty(model.StartTime) && !string.IsNullOrEmpty(model.EndTime))
            {
                var startParts = model.StartTime.Split(':');
                var endParts = model.EndTime.Split(':');

                var startMinutes = int.Parse(startParts[0]) * 60 + int.Parse(startParts[1]);
                var endMinutes = int.Parse(endParts[0]) * 60 + int.Parse(endParts[1]);

                if (endMinutes <= startMinutes)
                {
                    return BadRequest(new { message = "End time must be later than start time" });
                }
            }

            try
            {
                var startTimeParts = model.StartTime.Split(':');
                var startTime = new DateTime(1, 1, 1, int.Parse(startTimeParts[0]), int.Parse(startTimeParts[1]), 0, DateTimeKind.Utc);
                var endTimeParts = model.EndTime.Split(':');
                var endTime = new DateTime(1, 1, 1, int.Parse(endTimeParts[0]), int.Parse(endTimeParts[1]), 0, DateTimeKind.Utc);
                var entry = new TimetableEntry
                {
                    StudentId = model.StudentId,
                    ScheduleDate = DateTime.SpecifyKind(model.ScheduleDate.Date, DateTimeKind.Utc),
                    StartTime = startTime,
                    EndTime = endTime,
                    StatusId = model.StatusId,
                    Subject = model.Subject,
                    Location = model.Location,
                    Notes = model.Notes,
                    CreatedDate = DateTime.UtcNow
                };

                db.TimetableEntries.Add(entry);
                db.SaveChanges();

                return Ok(new { success = true, message = "Entry created successfully!", entryId = entry.EntryId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error creating entry: " + ex.Message });
            }
        }

        // DELETE: api/Timetable/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteEntry(int id)
        {
            bool isTeacher = HttpContext.Session.GetString("IsTeacher") == "true";
            if (!isTeacher)
                return Unauthorized(new { message = "Only teachers can delete entries" });

            try
            {
                var entry = db.TimetableEntries.Find(id);
                if (entry == null)
                    return NotFound(new { message = "Entry not found" });

                db.TimetableEntries.Remove(entry);
                db.SaveChanges();

                return Ok(new { success = true, message = "Entry deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error deleting entry: " + ex.Message });
            }
        }

        // Build Timetable ViewModel
        private TimetableViewModel BuildTimetableViewModel(int year, int month, bool isTeacher, int? studentId)
        {
            var firstDay = new DateTime(year, month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            var startDate = firstDay.AddDays(-(int)firstDay.DayOfWeek);
            if (firstDay.DayOfWeek == DayOfWeek.Sunday)
                startDate = firstDay.AddDays(-7);

            var endDate = lastDay.AddDays(6 - (int)lastDay.DayOfWeek);
            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            var entries = db.TimetableEntries
                .Include(t => t.Student)
                .Include(t => t.Status)
                .Where(t => t.ScheduleDate >= startDate && t.ScheduleDate <= endDate)
                .ToList();

            if (!isTeacher && studentId.HasValue)
                entries = entries.Where(e => e.StudentId == studentId.Value).ToList();

            var weeks = new List<WeekViewModel>();
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                var week = new WeekViewModel { Days = new List<DayViewModel>() };

                for (int i = 0; i < 7; i++)
                {
                    var dayEntries = entries
                        .Where(e => e.ScheduleDate.Date == currentDate.Date && e.Student != null && e.Status != null)
                        .Select(e => new TimetableEntryViewModel
                        {
                            EntryId = e.EntryId,
                            ScheduleDate = e.ScheduleDate,
                            StartTime = e.StartTime.ToString(@"HH\:mm") ?? "00:00",
                            EndTime = e.EndTime.ToString(@"HH\:mm") ?? "00:00",
                            TimeSlot = (e.StartTime.ToString(@"HH\:mm") ?? "00:00") + "-" + (e.EndTime.ToString(@"HH\:mm") ?? "00:00"),
                            StudentName = e.Student!.StudentName,
                            StudentCode = e.Student!.StudentCode,
                            Subject = e.Subject,
                            SubjectCode = !string.IsNullOrEmpty(e.Subject) && e.Subject.Length > 10
                                ? e.Subject.Substring(0, 10) + "..."
                                : e.Subject,
                            StatusName = e.Status!.StatusName,
                            ColorCode = e.Status!.ColorCode,
                            Location = e.Location,
                            Notes = e.Notes
                        })
                        .OrderBy(e => e.StartTime)
                        .ToList();

                    week.Days.Add(new DayViewModel
                    {
                        Date = currentDate,
                        DayOfMonth = currentDate.Day,
                        DayOfWeek = currentDate.DayOfWeek.ToString().Substring(0, 3),
                        IsCurrentMonth = currentDate.Month == month,
                        Entries = dayEntries
                    });

                    currentDate = currentDate.AddDays(1);
                }

                weeks.Add(week);
            }

            // Create StatusLegend using DTO without navigation properties to avoid circular references
            var statusLegend = db.ScheduleStatuses
                .AsNoTracking()
                .Select(s => new ScheduleStatusDto
                {
                    StatusId = s.StatusId,
                    StatusName = s.StatusName,
                    ColorCode = s.ColorCode,
                    Description = s.Description
                })
                .ToList();

            return new TimetableViewModel
            {
                Year = year,
                Month = month,
                MonthName = firstDay.ToString("MMMM yyyy") ?? $"{firstDay:MMMM yyyy}",
                Weeks = weeks,
                StatusLegend = statusLegend,
                IsTeacher = isTeacher,
                CurrentStudentId = studentId
            };
        }
    }
}