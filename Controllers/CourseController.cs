using BullDoghs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BullDoghs.Controllers
{
    public class CourseController : Controller

    {
        private readonly Team105DBContext _team105DBContext;

        public CourseController(Team105DBContext courseContext)
        {
            _team105DBContext = courseContext;
        }
        public IActionResult CourseView()
        {
            var courses = _team105DBContext.tbCourseSections.Include(p => p.CourseTitle_FKNavigation).Include(p => p.TeacherID_FKNavigation);
            return View(courses.ToList());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _team105DBContext.tbCourseSections == null)
            {
                return RedirectToAction("CourseView");
            }

            var courses = await _team105DBContext.tbCourseSections.Include(p => p.CourseTitle_FKNavigation).Include(p => p.TeacherID_FKNavigation)
                .FirstOrDefaultAsync(m => m.CourseSectionID_PK == id);

            if (courses == null)
            {
                return RedirectToAction("CourseView");
            }

            return View(courses);
        }

        [HttpGet]
        public IActionResult RegistrationView()
        {
            
            var registrations = _team105DBContext.tbRegistrations.Include(p => p.StudentID_FKNavigation).Include(p => p.CourseSectionID_FKNavigation).Include(p => p.CourseSectionID_FKNavigation.CourseTitle_FKNavigation);

            return View(registrations.ToList());
        }
        [HttpPost]
        public IActionResult RegistrationView(string courseName, string status, string studentFirstName)
        {
            var registrations = from p in _team105DBContext.tbRegistrations.Include(p => p.StudentID_FKNavigation).Include(p => p.CourseSectionID_FKNavigation).Include(p => p.CourseSectionID_FKNavigation.CourseTitle_FKNavigation) select p;
            
            ViewData["CourseNameFilter"] = courseName;
            ViewData["CourseStatusFilter"] = status;
            ViewData["StudentNameFilter"] = studentFirstName;


            if (!string.IsNullOrEmpty(courseName))
            {
                registrations = registrations.Where(p => p.CourseSectionID_FKNavigation.CourseTitle_FKNavigation.CourseTitle_PK.Contains(courseName));
            }

            if (!string.IsNullOrEmpty(status))
            {
                registrations = registrations.Where(p => p.CourseStatus.Equals(status));
            }

            if (!string.IsNullOrEmpty(studentFirstName))
            {
                registrations = registrations.Where(p => p.StudentID_FKNavigation.StudentName.Contains(studentFirstName));
            }


            return View(registrations.OrderBy(p => p.CourseRegistrationID_PK));
        }



    }
}
