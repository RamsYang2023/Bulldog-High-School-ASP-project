using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BullDoghs.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Build.Evaluation;

namespace BullDoghs.Controllers
{
    public class TeacherPortController : Controller
    {
        private readonly Team105DBContext _context;

        public TeacherPortController(Team105DBContext context)
        {
            _context = context;
        }

        // GET: TeacherPort
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Index()
        {
            String userName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;

            var team105DBContext = _context.tbCourseSections.Include(t => t.CourseTitle_FKNavigation).Include(t => t.TeacherID_FKNavigation).Where(u => u.TeacherID_FKNavigation.TeacherName == userName);
            return View(await team105DBContext.ToListAsync());
        }

        // GET: TeacherPort/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbCourseSections == null)
            {
                return NotFound();
            }

            var tbCourseSection = await _context.tbCourseSections
                .Include(t => t.CourseTitle_FKNavigation)
                .Include(t => t.TeacherID_FKNavigation)
                .FirstOrDefaultAsync(m => m.CourseSectionID_PK == id);
            if (tbCourseSection == null)
            {
                return NotFound();
            }

            return View(tbCourseSection);
        }

        // GET: TeacherPort/Create
        public IActionResult Create()
        {
            String userName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;

            ViewData["CourseTitle_FK"] = new SelectList(_context.tbCourses, "CourseTitle_PK", "CourseTitle_PK");
            ViewData["TeacherID_FK"] = new SelectList(_context.tbTeachers.Where(n => n.TeacherName == userName), "TeacherID_PK", "TeacherID_PK");
            return View();
        }

        // POST: TeacherPort/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseSectionID_PK,Course_day,Course_time,AttendentNumber,ClassRoom,TeacherID_FK,CourseTitle_FK")] tbCourseSection tbCourseSection)
        {
            
            if (ModelState.IsValid)
            {
                _context.Add(tbCourseSection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            String userName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;

            ViewData["CourseTitle_FK"] = new SelectList(_context.tbCourses, "CourseTitle_PK", "CourseTitle_PK", tbCourseSection.CourseTitle_FK);
            ViewData["TeacherID_FK"] = new SelectList(_context.tbTeachers.Where(n => n.TeacherName == userName), "TeacherID_PK", "TeacherID_PK", tbCourseSection.TeacherID_FK);
            return View(tbCourseSection);
        }

        // GET: TeacherPort/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbCourseSections == null)
            {
                return NotFound();
            }

            var tbCourseSection = await _context.tbCourseSections.FindAsync(id);
            if (tbCourseSection == null)
            {
                return NotFound();
            }
            ViewData["CourseTitle_FK"] = new SelectList(_context.tbCourses, "CourseTitle_PK", "CourseTitle_PK", tbCourseSection.CourseTitle_FK);
            ViewData["TeacherID_FK"] = new SelectList(_context.tbTeachers, "TeacherID_PK", "TeacherID_PK", tbCourseSection.TeacherID_FK);
            return View(tbCourseSection);
        }

        // POST: TeacherPort/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseSectionID_PK,Course_day,Course_time,AttendentNumber,ClassRoom,TeacherID_FK,CourseTitle_FK")] tbCourseSection tbCourseSection)
        {
            if (id != tbCourseSection.CourseSectionID_PK)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbCourseSection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tbCourseSectionExists(tbCourseSection.CourseSectionID_PK))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseTitle_FK"] = new SelectList(_context.tbCourses, "CourseTitle_PK", "CourseTitle_PK", tbCourseSection.CourseTitle_FK);
            //ViewData["TeacherID_FK"] = new SelectList(_context.tbTeachers, "TeacherID_PK", "TeacherID_PK", tbCourseSection.TeacherID_FK);
            return View(tbCourseSection);
        }

        // GET: TeacherPort/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbCourseSections == null)
            {
                return NotFound();
            }

            var tbCourseSection = await _context.tbCourseSections
                .Include(t => t.CourseTitle_FKNavigation)
                .Include(t => t.TeacherID_FKNavigation)
                .FirstOrDefaultAsync(m => m.CourseSectionID_PK == id);
            if (tbCourseSection == null)
            {
                return NotFound();
            }

            return View(tbCourseSection);
        }

        // POST: TeacherPort/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.tbCourseSections == null)
            {
                return Problem("Entity set 'Team105DBContext.tbCourseSections'  is null.");
            }
            var tbCourseSection = await _context.tbCourseSections.FindAsync(id);
            if (tbCourseSection != null)
            {
                _context.tbCourseSections.Remove(tbCourseSection);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool tbCourseSectionExists(int id)
        {
          return (_context.tbCourseSections?.Any(e => e.CourseSectionID_PK == id)).GetValueOrDefault();
        }
    }
}
