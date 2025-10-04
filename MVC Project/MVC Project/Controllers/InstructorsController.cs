using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_Project.Interfaces;
using MVC_Project.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MVC_Project.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly IInstructorService _instructorService;
        private readonly ICourseService _courseService;
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<InstructorsController> _logger;

        public InstructorsController(
            IInstructorService instructorService,
            ICourseService courseService,
            IDepartmentService departmentService,
            ILogger<InstructorsController> logger)
        {
            _instructorService = instructorService;
            _courseService = courseService;
            _departmentService = departmentService;
            _logger = logger;
        }

        // GET: Instructors
        public async Task<IActionResult> Index()
        {
            var instructors = await _instructorService.GetAllAsync();
            return View(instructors);
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _instructorService.GetByIdAsync(id.Value);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructors/Create
        public async Task<IActionResult> Create()
        {
            var departments = await _departmentService.GetAllAsync();
            var courses = await _courseService.GetAllAsync();
            _logger.LogInformation("Departments count: {Count}, Courses count: {Count}", departments.Count(), courses.Count());
            if (!departments.Any())
            {
                ModelState.AddModelError("", "No departments available. Please add departments first.");
            }
            if (!courses.Any())
            {
                ModelState.AddModelError("", "No courses available. Please add courses first.");
            }
            ViewData["CourseId"] = new SelectList(courses, "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(departments, "Id", "Name");
            return View();
        }

        // POST: Instructors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Address,Salary,Image,DepartmentId,CourseId")] Instructor instructor)
        {
            _logger.LogInformation("Create attempt: Name={Name}, Image={Image}, DepartmentId={DepartmentId}, CourseId={CourseId}",
                instructor.Name, instructor.Image, instructor.DepartmentId, instructor.CourseId);

            // Check if IDs exist in the database
            var departments = await _departmentService.GetAllAsync();
            var courses = await _courseService.GetAllAsync();
            if (!departments.Any(d => d.Id == instructor.DepartmentId))
            {
                ModelState.AddModelError("DepartmentId", $"Department ID {instructor.DepartmentId} does not exist.");
                _logger.LogWarning("Invalid DepartmentId: {DepartmentId}", instructor.DepartmentId);
            }
            if (!courses.Any(c => c.Id == instructor.CourseId))
            {
                ModelState.AddModelError("CourseId", $"Course ID {instructor.CourseId} does not exist.");
                _logger.LogWarning("Invalid CourseId: {CourseId}", instructor.CourseId);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("ModelState is invalid. Errors: {Errors}", string.Join("; ", errors));
                ViewData["ValidationErrors"] = string.Join("<br>", errors);
                ViewData["CourseId"] = new SelectList(await _courseService.GetAllAsync(), "Id", "Name", instructor.CourseId);
                ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name", instructor.DepartmentId);
                return View(instructor);
            }

            try
            {
                await _instructorService.AddAsync(instructor);
                if (instructor.Id == 0)
                {
                    throw new InvalidOperationException("Instructor ID was not assigned after save.");
                }
                _logger.LogInformation("Instructor created successfully with ID: {Id}", instructor.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error: {Message}", ex.InnerException?.Message);
                ModelState.AddModelError("", "Database error: Unable to save. Check Department and Course IDs. Details: " + ex.InnerException?.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Save operation failed: {Message}", ex.Message);
                ModelState.AddModelError("", "Save failed: " + ex.Message + ". Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                ModelState.AddModelError("", "An unexpected error occurred. Try again. Details: " + ex.Message);
            }

            ViewData["CourseId"] = new SelectList(await _courseService.GetAllAsync(), "Id", "Name", instructor.CourseId);
            ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name", instructor.DepartmentId);
            return View(instructor);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _instructorService.GetByIdAsync(id.Value);
            if (instructor == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(await _courseService.GetAllAsync(), "Id", "Name", instructor.CourseId);
            ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name", instructor.DepartmentId);
            return View(instructor);
        }

        // POST: Instructors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Salary,Image,DepartmentId,CourseId")] Instructor instructor)
        {
            if (id != instructor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _instructorService.UpdateAsync(instructor);
                    _logger.LogInformation("Instructor updated successfully with ID: {Id}", instructor.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    _logger.LogError("DbUpdateConcurrencyException occurred");
                    ModelState.AddModelError("", "Unable to save changes. The instructor was deleted by another user.");
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database update error: {Message}", ex.InnerException?.Message);
                    ModelState.AddModelError("", "Database error: Unable to save. Check Department and Course IDs. Details: " + ex.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                    ModelState.AddModelError("", "An unexpected error occurred. Try again. Details: " + ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("ModelState is invalid. Errors: {Errors}", string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            }

            ViewData["CourseId"] = new SelectList(await _courseService.GetAllAsync(), "Id", "Name", instructor.CourseId);
            ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name", instructor.DepartmentId);
            return View(instructor);
        }

        // GET: Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _instructorService.GetByIdAsync(id.Value);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _instructorService.DeleteAsync(id);
                _logger.LogInformation("Instructor deleted successfully with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting instructor: {Message}", ex.Message);
                ModelState.AddModelError("", "Unable to delete. Try again. Details: " + ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}