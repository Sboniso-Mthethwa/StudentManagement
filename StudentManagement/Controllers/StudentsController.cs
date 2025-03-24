using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
   
    public class StudentsController : Controller
    {
        private StudentDbContext db = new StudentDbContext();

        // GET: Students
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Students.ToList());
        }

        // GET: Students/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Streams = GetStreamCourses(); // Fetch streams and courses
            return View();
        }

        [Authorize]
        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StudentId,IdNumber,Name,Email,PhoneNumber,SelectedStream,SelectedCourses")] Student student, int[] SelectedCourses)
        {
            if (ModelState.IsValid)
            {
                // Check if ID Number already exists
                if (db.Students.Any(s => s.IdNumber == student.IdNumber))
                {
                    ModelState.AddModelError("IdNumber", "The student is already registered with this ID Number.");
                    ViewBag.Streams = GetStreamCourses(); // Re-populate streams on error
                    return View(student);
                }

                // Check if Email already exists
                if (db.Students.Any(s => s.Email == student.Email))
                {
                    ModelState.AddModelError("Email", "The email address is already taken.");
                    ViewBag.Streams = GetStreamCourses(); // Re-populate streams on error
                    return View(student);
                }

                db.Students.Add(student);
                db.SaveChanges();

                // Add selected courses for the student
                if (SelectedCourses != null)
                {
                    foreach (var courseId in SelectedCourses)
                    {
                        var mark = new Mark
                        {
                            StudentId = student.StudentId,
                            CourseId = courseId,
                            Assignment1 = 0, // Default score (can be updated later)
                            Score = 0 // Default test score (can be updated later)
                        };
                        db.Marks.Add(mark);
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            ViewBag.Streams = GetStreamCourses(); // Re-populate streams on error
            return View(student);
        }

        #region Helper method to get streams and their courses
        private List<Stream> GetStreamCourses()
        {
            return new List<Stream>
            {
                new Stream
                {
                    StreamName = "General",
                    Courses = new List<Course>
                    {
                         new Course { CourseId = 23, CourseName = "Mathematics Literacy", Credits = 7 },
                        new Course { CourseId = 4, CourseName = "English", Credits = 7 },
                        new Course { CourseId = 21, CourseName = "Life Orientation", Credits = 7 },
                       new Course { CourseId = 22, CourseName = "IsiZulu", Credits = 7 },
                       new Course { CourseId = 25, CourseName = "Geography", Credits = 7 },
                       new Course { CourseId = 26, CourseName = "History", Credits = 7 },
                       new Course { CourseId = 30, CourseName = "Tourism", Credits = 7 },
                        new Course { CourseId = 5, CourseName = "Afrikaans", Credits = 7 },
                    }
                },
                new Stream
                {
                    StreamName = "Physics",
                    Courses = new List<Course>
                    {
                        new Course { CourseId = 24, CourseName = "Physical Sciences", Credits = 7 },
                        new Course { CourseId = 14, CourseName = "Life Sciences", Credits = 7 },
                        new Course { CourseId = 31, CourseName = "Information Technology", Credits = 7 },
                        new Course { CourseId = 6, CourseName = "Mathematics", Credits = 7 },
                        new Course { CourseId = 4, CourseName = "English", Credits = 7 },
                        new Course { CourseId = 21, CourseName = "Life Orientation", Credits = 7 },
                        new Course { CourseId = 22, CourseName = "IsiZulu", Credits = 7 },
                         new Course { CourseId = 5, CourseName = "Afrikaans", Credits = 7 },
                    }
                },
                new Stream
                {
                    StreamName = "Accounting",
                    Courses = new List<Course>
                    {
                        new Course { CourseId = 6, CourseName = "Mathematics", Credits = 7 },
                        new Course { CourseId = 4, CourseName = "English", Credits = 7 },
                        new Course { CourseId = 21, CourseName = "Life Orientation", Credits = 7 },
                       new Course { CourseId = 22, CourseName = "IsiZulu", Credits = 7 },
                        new Course { CourseId = 27, CourseName = "Accounting", Credits = 7 },
                        new Course { CourseId = 28, CourseName = "Economics", Credits = 7 },
                        new Course { CourseId = 29, CourseName = "Agricultural Sciences", Credits = 7 },
                         new Course { CourseId = 5, CourseName = "Afrikaans", Credits = 7 },
                    }
                }
            };
        }
        #endregion


        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Student student = db.Students.Include(s => s.Marks.Select(m => m.Course)).SingleOrDefault(s => s.StudentId == id);

            if (student == null)
            {
                return HttpNotFound();
            }

            // Fetch the selected stream (Assuming StreamName is a property in your Stream class)
            student.SelectedStream = GetSelectedStreamName(student.SelectedStream);

            // Pass the courses associated with the student to the view
            ViewBag.RegisteredCourses = student.Marks.Select(m => m.Course.CourseName).ToList();
            return View(student);
        }

        private string GetSelectedStreamName(string streamId)
        {
            // Fetch the stream name based on the selected stream ID or name
            // Implement your logic to return the correct stream name based on your data structure
            // Here’s a mockup logic to fetch stream name
            var stream = db.Streams.FirstOrDefault(s => s.StreamId.ToString() == streamId);
            return stream != null ? stream.StreamName : string.Empty;
        }


        [Authorize]
        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudentId,Name,Email,PhoneNumber")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }
        [Authorize]
        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }
        [Authorize]
        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
    }
}
