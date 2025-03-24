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
    public class MarksController : Controller
    {
        private StudentDbContext db = new StudentDbContext();
        [Authorize]
        // GET: Marks
        public ActionResult Index()
        {
            var marks = db.Marks.Include(m => m.Course).Include(m => m.Student);
            return View(marks.ToList());
        }
        [Authorize]
        // GET: Marks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mark mark = db.Marks.Find(id);
            if (mark == null)
            {
                return HttpNotFound();
            }
            return View(mark);
        }

        [Authorize]
        // GET: Marks/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "Name");
            return View();
        }

        [Authorize]
        // POST: Marks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MarkId,StudentId,CourseId,Assignment1,Score")] Mark mark)
        {
            if (ModelState.IsValid)
            {
                // Check if marks already exist for the given StudentId and CourseId
                var existingMark = db.Marks
                                     .FirstOrDefault(m => m.StudentId == mark.StudentId && m.CourseId == mark.CourseId);

                if (existingMark != null)
                {
                    // If marks already exist, add an error to the ModelState
                    ModelState.AddModelError("", "Marks for this student and subject have already been entered.");
                }
                else
                {
                    // If no existing marks, proceed to add the new marks
                    db.Marks.Add(mark);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", mark.CourseId);
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "Name", mark.StudentId);
            return View(mark);
        }

        [Authorize]
        // GET: Marks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mark mark = db.Marks.Find(id);
            if (mark == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", mark.CourseId);
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "Name", mark.StudentId);
            return View(mark);
        }

        [Authorize]
        // POST: Marks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MarkId,StudentId,CourseId,Assignment1,Score")] Mark mark)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mark).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", mark.CourseId);
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "Name", mark.StudentId);
            return View(mark);
        }

        // GET: Marks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mark mark = db.Marks.Find(id);
            if (mark == null)
            {
                return HttpNotFound();
            }
            return View(mark);
        }

        // POST: Marks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Mark mark = db.Marks.Find(id);
            db.Marks.Remove(mark);
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

        public ActionResult ViewResult(string studentName)
        {
            // Fetch records based on student name
            var marks = db.Marks
                .Include(m => m.Student)
                .Include(m => m.Course)
                .Where(m => m.Student.Name.Contains(studentName))
                .ToList();

            // Calculate average score and pass/fail status
            var averageScore = marks.Count > 0 ? marks.Average(m => (m.Assignment1 * 0.20) + (m.Score * 0.80)) : 0;
            var passStatus = averageScore >= 50 ? "Pass" : "Fail";

            // Pass data to the view
            ViewBag.AverageScore = averageScore.ToString("F2");
            ViewBag.PassStatus = passStatus;

            return View(marks);
        }


        [Authorize]
        public ActionResult PassFailChart()
        {
            // Fetch all student marks grouped by student
            var studentMarks = db.Marks.Include(m => m.Student)
                                       .GroupBy(m => m.StudentId)
                                       .Select(g => new
                                       {
                                           StudentId = g.Key,
                                           StudentName = g.FirstOrDefault().Student.Name,
                                           AverageScore = g.Average(m => (m.Assignment1 * 0.20) + (m.Score * 0.80))
                                       }).ToList();

            // Count students who passed and failed
            var passCount = studentMarks.Count(s => s.AverageScore >= 50);
            var failCount = studentMarks.Count(s => s.AverageScore < 50);

            // Pass data to the view
            ViewBag.PassCount = passCount;
            ViewBag.FailCount = failCount;

            return View();
        }
        //public ActionResult SubjectChart()
        //{
        //    return View();
        //}

        //[HttpPost]

        //[Authorize]
        //public ActionResult SubjectChart(string subjectName)
        //{

        //    // Find the course based on the provided subject name
        //    var course = db.Courses.FirstOrDefault(c => c.CourseName.Equals(subjectName, StringComparison.OrdinalIgnoreCase));

        //    if (course == null)
        //    {
        //        ViewBag.Message = "Subject not found.";
        //        return View();
        //    }

        //    int courseId = course.CourseId; // Get the ID of the found course
        //    var marks = db.Marks.Include(m => m.Course).Where(m => m.CourseId == courseId).ToList();

        //    if (!marks.Any())
        //    {
        //        ViewBag.Message = "No marks available for this subject.";
        //        return View();
        //    }

        //    // Calculate pass and fail counts
        //    var passCount = marks.Count(m => (m.Assignment1 * 0.20 + m.Score * 0.80) >= 50);
        //    var failCount = marks.Count(m => (m.Assignment1 * 0.20 + m.Score * 0.80) < 50);

        //    // Pass data to the view
        //    ViewBag.CourseName = course.CourseName; // Subject name
        //    ViewBag.PassCount = passCount;
        //    ViewBag.FailCount = failCount;

        //    return View("SubjectChart"); // Return the view with the chart
        //}
        [Authorize]
        public ActionResult SubjectChart()
        {
            var courseDataList = new List<CoursePassFailViewModel>(); 

            // Populate courseDataList with course names and pass/fail counts
            var courses = db.Courses.Include(c => c.Marks).ToList();

            foreach (var course in courses)
            {
                var marks = course.Marks.ToList();
                var passCount = marks.Count(m => (m.Assignment1 * 0.20 + m.Score * 0.80) >= 50);
                var failCount = marks.Count(m => (m.Assignment1 * 0.20 + m.Score * 0.80) < 50);

                courseDataList.Add(new CoursePassFailViewModel
                {
                    CourseName = course.CourseName,
                    PassCount = passCount,
                    FailCount = failCount
                });
            }

            ViewBag.CoursePassFailData = courseDataList; // Pass the data to the view

            return View();
        }



    }
}

