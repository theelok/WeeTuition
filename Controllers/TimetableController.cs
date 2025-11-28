using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimetableSystem.Models;

namespace TimetableSystem.Controllers
{
    public class TimetableController : Controller
    {
        private readonly AppDbContext db;

        public TimetableController(AppDbContext context)
        {
            db = context;
        }

        // GET: Timetable/Index
        public IActionResult Index(int? year, int? month)
        {
            bool isTeacher = HttpContext.Session.GetString("IsTeacher") == "true";
            int? studentId = HttpContext.Session.GetInt32("StudentId");

            if (!isTeacher && !studentId.HasValue)
                return RedirectToAction("Login", "Account");

            if (!year.HasValue || !month.HasValue)
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
            }

            var viewModel = BuildTimetableViewModel(year.Value, month.Value, isTeacher, studentId);
            return View(viewModel);
        }

        // GET: Timetable/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            bool isTeacher = HttpContext.Session.GetString("IsTeacher") == "true";
            if (!isTeacher)
                return RedirectToAction("Index");

            var entry = db.TimetableEntries
                .Include(t => t.Student)
                .Include(t => t.Status)
                .FirstOrDefault(t => t.EntryId == id);

            if (entry == null)
                return NotFound();

            var viewModel = new EditEntryViewModel
            {
                EntryId = entry.EntryId,
                StudentId = entry.StudentId,
                ScheduleDate = entry.ScheduleDate,
                StartTime = entry.StartTime.ToString(@"HH\:mm"),
                EndTime = entry.EndTime.ToString(@"HH\:mm"),
                StatusId = entry.StatusId,
                Subject = entry.Subject,
                Location = entry.Location,
                Notes = entry.Notes,
                Students = db.Students.ToList(),
                Statuses = db.ScheduleStatuses.ToList()
            };

            return View(viewModel);
        }

        // POST: Timetable/Edit
        // POST: Timetable/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditEntryViewModel model)
        {
            bool isTeacher = HttpContext.Session.GetString("IsTeacher") == "true";
            if (!isTeacher)
                return RedirectToAction("Index");

            // Validate time
            if (!string.IsNullOrEmpty(model.StartTime) && !string.IsNullOrEmpty(model.EndTime))
            {
                var startParts = model.StartTime.Split(':');
                var endParts = model.EndTime.Split(':');

                var startMinutes = int.Parse(startParts[0]) * 60 + int.Parse(startParts[1]);
                var endMinutes = int.Parse(endParts[0]) * 60 + int.Parse(endParts[1]);

                if (endMinutes <= startMinutes)
                {
                    ModelState.AddModelError("EndTime", "End time must be later than start time");
                    model.Students = db.Students.ToList();
                    model.Statuses = db.ScheduleStatuses.ToList();
                    return View(model);
                }
            }

            try
            {
                // Fetch the entry WITH tracking (remove AsNoTracking())
                var entry = db.TimetableEntries.FirstOrDefault(t => t.EntryId == model.EntryId);
                if (entry == null)
                    return NotFound();

                // Update the tracked entity
                entry.StudentId = model.StudentId;
                entry.ScheduleDate = DateTime.SpecifyKind(model.ScheduleDate.Date, DateTimeKind.Utc);

                // Parse time correctly
                var startTimeParts = model.StartTime.Split(':');
                entry.StartTime = new DateTime(1, 1, 1, int.Parse(startTimeParts[0]), int.Parse(startTimeParts[1]), 0, DateTimeKind.Utc);

                var endTimeParts = model.EndTime.Split(':');
                entry.EndTime = new DateTime(1, 1, 1, int.Parse(endTimeParts[0]), int.Parse(endTimeParts[1]), 0, DateTimeKind.Utc);

                entry.StatusId = model.StatusId;
                entry.Subject = model.Subject;
                entry.Location = model.Location;
                entry.Notes = model.Notes;

                // No need to call Update() - the entity is already being tracked
                db.SaveChanges();

                TempData["SuccessMessage"] = "Entry updated successfully!";
                return RedirectToAction("Index", new { year = model.ScheduleDate.Year, month = model.ScheduleDate.Month });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving changes: " + ex.Message);
                model.Students = db.Students.ToList();
                model.Statuses = db.ScheduleStatuses.ToList();
                return View(model);
            }
        }

        // GET: Timetable/Create
        [HttpGet]
        public IActionResult Create(DateTime? date)
        {
            bool isTeacher = HttpContext.Session.GetString("IsTeacher") == "true";
            if (!isTeacher)
                return RedirectToAction("Index");

            var viewModel = new EditEntryViewModel
            {
                ScheduleDate = date ?? DateTime.Today,
                StartTime = "09:00",
                EndTime = "10:00",
                StatusId = 1,
                Students = db.Students.ToList(),
                Statuses = db.ScheduleStatuses.ToList()
            };

            return View(viewModel);
        }

        // POST: Timetable/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EditEntryViewModel model)
        {
            bool isTeacher = HttpContext.Session.GetString("IsTeacher") == "true";
            if (!isTeacher)
                return RedirectToAction("Index");

            // Validate time
            if (!string.IsNullOrEmpty(model.StartTime) && !string.IsNullOrEmpty(model.EndTime))
            {
                var startParts = model.StartTime.Split(':');
                var endParts = model.EndTime.Split(':');

                var startMinutes = int.Parse(startParts[0]) * 60 + int.Parse(startParts[1]);
                var endMinutes = int.Parse(endParts[0]) * 60 + int.Parse(endParts[1]);

                if (endMinutes <= startMinutes)
                {
                    ModelState.AddModelError("EndTime", "End time must be later than start time");
                    model.Students = db.Students.ToList();
                    model.Statuses = db.ScheduleStatuses.ToList();
                    return View(model);
                }
            }

            try
            {
                // Parse time correctly
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

                TempData["SuccessMessage"] = "Entry created successfully!";
                return RedirectToAction("Index", new { year = model.ScheduleDate.Year, month = model.ScheduleDate.Month });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating entry: " + ex.Message);
                model.Students = db.Students.ToList();
                model.Statuses = db.ScheduleStatuses.ToList();
                return View(model);
            }
        }

        // POST: Timetable/Delete
        [HttpPost]
        public JsonResult Delete(int id)
        {
            bool isTeacher = HttpContext.Session.GetString("IsTeacher") == "true";
            if (!isTeacher)
                return Json(new { success = false, message = "Unauthorized" });

            try
            {
                var entry = db.TimetableEntries.Find(id);
                if (entry == null)
                    return Json(new { success = false, message = "Entry not found" });

                db.TimetableEntries.Remove(entry);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting entry: " + ex.Message });
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
                        .Where(e => e.ScheduleDate.Date == currentDate.Date)
                        .Select(e => new TimetableEntryViewModel
                        {
                            EntryId = e.EntryId,
                            ScheduleDate = e.ScheduleDate,
                            StartTime = e.StartTime.ToString(@"HH\:mm"),
                            EndTime = e.EndTime.ToString(@"HH\:mm"),
                            TimeSlot = e.StartTime.ToString(@"HH\:mm") + "-" + e.EndTime.ToString(@"HH\:mm"),
                            StudentName = e.Student.StudentName,
                            StudentCode = e.Student.StudentCode,
                            Subject = e.Subject,
                            SubjectCode = !string.IsNullOrEmpty(e.Subject) && e.Subject.Length > 10
                                ? e.Subject.Substring(0, 10) + "..."
                                : e.Subject,
                            StatusName = e.Status.StatusName,
                            ColorCode = e.Status.ColorCode,
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

            return new TimetableViewModel
            {
                Year = year,
                Month = month,
                MonthName = firstDay.ToString("MMMM yyyy"),
                Weeks = weeks,
                StatusLegend = db.ScheduleStatuses.ToList(),
                IsTeacher = isTeacher,
                CurrentStudentId = studentId
            };
        }
    }
}