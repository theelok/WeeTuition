using Microsoft.AspNetCore.Mvc;
using TimetableSystem.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace TimetableSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext db;

        public AccountController(AppDbContext context)
        {
            db = context;
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Teacher login (hardcoded)
            if (model.Username == "teacher" && model.Password == "teacher123")
            {
                HttpContext.Session.SetString("IsTeacher", "true");
                HttpContext.Session.SetString("Username", "Teacher");
                HttpContext.Session.Remove("StudentId");

                return RedirectToAction("Index", "Timetable");
            }

            // Student login (from database)
            var student = db.Students
                .FirstOrDefault(s => s.Username == model.Username && s.Password == model.Password);

            if (student != null)
            {
                HttpContext.Session.SetString("IsTeacher", "false");
                HttpContext.Session.SetInt32("StudentId", student.StudentId);
                HttpContext.Session.SetString("Username", student.StudentName);

                return RedirectToAction("Index", "Timetable");
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
