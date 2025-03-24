using Microsoft.AspNet.Identity;
using StudentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace StudentManagement.Controllers
{
    public class HomeController : Controller
    {
        private StudentDbContext db = new StudentDbContext();

        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult AdminDashboard()
        {
            var students = db.Students.ToList();

            // Check if students list is null or empty
            if (students == null || !students.Any())
            {
                ViewBag.Message = "No students found in the database.";
                return View(new List<Student>()); // Pass an empty list to the view
            }

            return View(students);

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult DisplayLearnerRecord()
        {
            var userEmail = User.Identity.Name; // Logged-in user's email
            var student = db.Students.FirstOrDefault(s => s.Email == userEmail); 

            if (student != null)
            {
                var marks = db.Marks
                    .Include(m => m.Course)
                    .Where(m => m.StudentId == student.StudentId) 
                    .ToList();

                // Calculate average score and pass/fail status
                var averageScore = marks.Count > 0 ? marks.Average(m => (m.Assignment1 * 0.20) + (m.Score * 0.80)) : 0;
                var passStatus = averageScore >= 50 ? "Pass" : "Fail";

                // Pass data to the view
                ViewBag.AverageScore = averageScore.ToString("F2"); // Format to two decimal places
                ViewBag.PassStatus = passStatus;

                ViewBag.Student = student; // Pass student information to the view


                return View(marks); // Return marks
            }

            return HttpNotFound("Student not found."); // Handle case where no student is found
        }


    }
}