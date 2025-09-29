using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MVC_Project.Controllers
{
    public class CourseStudentsController : Controller
    {
        private readonly Context _context;

        public CourseStudentsController(Context context)
        {
            _context = context;
        }

        // GET: CourseStudents
        public async Task<IActionResult> Index()
        {
            var context = _context.CourseStudents.Include(c => c.Course).Include(c => c.Student);
            return View(await context.ToListAsync());
        }

        // GET: CourseStudents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseStudents = await _context.CourseStudents
                .Include(c => c.Course)
                .Include(c => c.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseStudents == null)
            {
                return NotFound();
            }

            return View(courseStudents);
        }

        // GET: CourseStudents/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id");
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id");
            return View();
        }

        // POST: CourseStudents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Degree,CourseId,StudentId")] CourseStudents courseStudents)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courseStudents);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", courseStudents.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", courseStudents.StudentId);
            return View(courseStudents);
        }

        // GET: CourseStudents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseStudents = await _context.CourseStudents.FindAsync(id);
            if (courseStudents == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", courseStudents.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", courseStudents.StudentId);
            return View(courseStudents);
        }

        // POST: CourseStudents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Degree,CourseId,StudentId")] CourseStudents courseStudents)
        {
            if (id != courseStudents.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseStudents);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseStudentsExists(courseStudents.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", courseStudents.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", courseStudents.StudentId);
            return View(courseStudents);
        }

        // GET: CourseStudents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseStudents = await _context.CourseStudents
                .Include(c => c.Course)
                .Include(c => c.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseStudents == null)
            {
                return NotFound();
            }

            return View(courseStudents);
        }

        // POST: CourseStudents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseStudents = await _context.CourseStudents.FindAsync(id);
            if (courseStudents != null)
            {
                _context.CourseStudents.Remove(courseStudents);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseStudentsExists(int id)
        {
            return _context.CourseStudents.Any(e => e.Id == id);
        }
    }
}
