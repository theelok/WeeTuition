using Microsoft.AspNetCore.Mvc;
using TimetableSystem.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace TimetableSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext db;

        public AccountController(AppDbContext context)
        {
            db = context;
        }

        // POST: api/Account/Login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input" });

            // Teacher login (hardcoded)
            if (model.Username == "teacher" && model.Password == "teacher123")
            {
                HttpContext.Session.SetString("IsTeacher", "true");
                HttpContext.Session.SetString("Username", "Teacher");
                HttpContext.Session.Remove("StudentId");

                // Verify session was set
                var testValue = HttpContext.Session.GetString("IsTeacher");
                
                var response = Ok(new { 
                    success = true, 
                    isTeacher = true, 
                    username = "Teacher",
                    studentId = (int?)null,
                    sessionSet = testValue == "true"
                });
                
                return response;
            }

            // Student login (from database)
            var student = db.Students
                .FirstOrDefault(s => s.Username == model.Username && s.Password == model.Password);

            if (student != null)
            {
                HttpContext.Session.SetString("IsTeacher", "false");
                HttpContext.Session.SetInt32("StudentId", student.StudentId);
                HttpContext.Session.SetString("Username", student.StudentName);

                return Ok(new { 
                    success = true, 
                    isTeacher = false, 
                    username = student.StudentName,
                    studentId = student.StudentId
                });
            }

            return Unauthorized(new { message = "Invalid username or password" });
        }

        // POST: api/Account/Logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { success = true });
        }

        // GET: api/Account/CurrentUser
        [HttpGet("current")]
        public IActionResult GetCurrentUser()
        {
            var isTeacher = HttpContext.Session.GetString("IsTeacher") == "true";
            var studentId = HttpContext.Session.GetInt32("StudentId");
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { message = "Not logged in" });

            return Ok(new { 
                isTeacher, 
                studentId, 
                username 
            });
        }

        // GET: api/Account/TestSession
        [HttpGet("testsession")]
        public IActionResult TestSession()
        {
            var sessionId = HttpContext.Session.Id;
            var isTeacher = HttpContext.Session.GetString("IsTeacher");
            var hasSession = !string.IsNullOrEmpty(sessionId);
            
            return Ok(new { 
                hasSession,
                sessionId,
                isTeacher,
                cookieHeader = Request.Headers["Cookie"].ToString()
            });
        }
    }
}
