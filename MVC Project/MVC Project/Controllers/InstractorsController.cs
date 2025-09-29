using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MVC_Project.Controllers
{
    public class InstractorsController : Controller
    {
        private readonly Context _context;

        public InstractorsController(Context context)
        {
            _context = context;
        }

        // GET: Instractors
        public async Task<IActionResult> Index()
        {
            var context = _context.Instractors.Include(i => i.Course).Include(i => i.Department);
            return View(await context.ToListAsync());
        }

        // GET: Instractors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instractor = await _context.Instractors
                .Include(i => i.Course)
                .Include(i => i.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instractor == null)
            {
                return NotFound();
            }

            return View(instractor);
        }

        // GET: Instractors/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id");
            return View();
        }

        // POST: Instractors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Salary,Image,DepartmentId,CourseId")] Instractor instractor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instractor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", instractor.CourseId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", instractor.DepartmentId);
            return View(instractor);
        }

        // GET: Instractors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instractor = await _context.Instractors.FindAsync(id);
            if (instractor == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", instractor.CourseId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", instractor.DepartmentId);
            return View(instractor);
        }

        // POST: Instractors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Salary,Image,DepartmentId,CourseId")] Instractor instractor)
        {
            if (id != instractor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instractor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstractorExists(instractor.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", instractor.CourseId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", instractor.DepartmentId);
            return View(instractor);
        }

        // GET: Instractors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instractor = await _context.Instractors
                .Include(i => i.Course)
                .Include(i => i.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instractor == null)
            {
                return NotFound();
            }

            return View(instractor);
        }

        // POST: Instractors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instractor = await _context.Instractors.FindAsync(id);
            if (instractor != null)
            {
                _context.Instractors.Remove(instractor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstractorExists(int id)
        {
            return _context.Instractors.Any(e => e.Id == id);
        }
    }
}
