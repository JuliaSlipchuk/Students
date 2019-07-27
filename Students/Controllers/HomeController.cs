using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Students.Models;
using System.Text.RegularExpressions;
using System.IO;
using System.Data.Entity;

namespace Students.Controllers
{
    public class HomeController : Controller
    {
        public static List<string> courses = new List<string>();
        private StudentContext db = new StudentContext();
        static bool check = false;

        [HttpGet]
        public ActionResult Index()
        {
            if (!check)
            {
                FillCourses();
                check = true;
            }
            ViewBag.Students = db.Students.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Index(string login, string password, string buttVal)
        {
            if (login == "July" && password == "Originals")
            {
                return RedirectToRoute(new { controller = "Home", action = "AddEditDelete",  method = buttVal});
            }
            return HttpNotFound();
        }
        private static void FillCourses()
        {
            string[] mass = { "Data structure", ".NET", "HTML/CSS/JS", "Mathematica" };
            courses.AddRange(mass);
        }

        [HttpGet]
        public ActionResult AddEditDelete(string method, int? id)
        {
            ViewBag.Courses = courses;
            ViewBag.id = id;
            Regex regex = new Regex(@"(((http)|(https))://((\d|\w|\.|:)+)(/|(/Student)|(/Student/Index)))$");
            if (Request?.UrlReferrer != null || regex.IsMatch(Request.UrlReferrer.OriginalString))
            {
                return View(method);
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult AddEditDelete(Student student, string buttonValue, HttpPostedFileBase uploadImage, string[] CoursesList)
        {
            Regex email = new Regex(@"^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$");
            if (buttonValue == "Delete")
            {
                Student stud = db.Students.Find(student.Id);
                if (stud != null)
                {
                    db.Students.Remove(stud);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else if (buttonValue == "Exit")
            {
                return RedirectToAction("Index");
            }
            if (email != null && email.IsMatch(student.Email))
            {
                if (buttonValue == "Add")
                {
                    ForImg(student, uploadImage);
                    ForCourses(student, CoursesList);
                    db.Students.Add(student);
                }
                else if (buttonValue == "Edit")
                {
                    db.Entry(student).State = EntityState.Modified;
                    ForImg(student, uploadImage);
                    ForCourses(student, CoursesList);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return Content("<h2>You entered invalid email!</h2>");
        }

        [HttpPost]
        public ActionResult AddRemoveCourse(string addCourses, string[] CoursesList, string what)
        {
            if (what == "Add")
            {
                string[] newCourses = addCourses.Split(' ');
                for (int i = 0; i < newCourses.Length; i++)
                {
                    courses.Add(newCourses[i]);
                }
            }
            else if (what == "Exit")
            {
                return RedirectToAction("Index");
            }
            else
            {
                if (CoursesList != null)
                {
                    for (int i = 0; i < CoursesList.Length; i++)
                    {
                        courses.Remove(CoursesList[i]);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public void ForImg(Student student, HttpPostedFileBase uploadImage)
        {
            if (uploadImage != null)
            {
                int start = uploadImage.FileName.LastIndexOf('.');
                student.Extension = uploadImage.FileName.Substring(start + 1).ToLower();
                if (student.Extension == "jpg" || student.Extension == "png" || student.Extension == "gif")
                {
                    using (BinaryReader reader = new BinaryReader(uploadImage.InputStream))
                    {
                        student.Image = reader.ReadBytes(uploadImage.ContentLength);
                    }
                }
            }
        }
        public void ForCourses(Student student, string[] CoursesList)
        {
            if (CoursesList != null && CoursesList.Length != 0)
            {
                string courses = "";
                for (int i = 0; i < CoursesList.Length; i++)
                {
                    if (i != CoursesList.Length - 1)
                    {
                        courses += CoursesList[i] + "|";
                    }
                    else
                    {
                        courses += CoursesList[i];
                    }
                }
                student.Courses = courses;
            }
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
    }
}