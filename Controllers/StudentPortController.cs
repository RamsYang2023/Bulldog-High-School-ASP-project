using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BullDoghs.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BullDoghs.Controllers
{
    public class StudentPortController : Controller
    {
        private readonly Team105DBContext _context;

        public StudentPortController(Team105DBContext context)
        {
            _context = context;
        }

        // GET: StudentPort
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentView()
        {
            //int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            String userName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;

            var team105DBContext = _context.tbRegistrations.Include(t => t.CourseSectionID_FKNavigation).Include(t => t.CourseSectionID_FKNavigation.TeacherID_FKNavigation).Include(t => t.CourseSectionID_FKNavigation.CourseTitle_FKNavigation).Include(t => t.StudentID_FKNavigation).Where(u => u.StudentID_FKNavigation.StudentName == userName);

            return View(await team105DBContext.ToListAsync());
        }

        
    }
}
