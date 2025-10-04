using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_Project.Interfaces;
using MVC_Project.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MVC_Project.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ICourseService courseService, IDepartmentService departmentService, ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _departmentService = departmentService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAllAsync();
            return View(courses);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseService.GetByIdAsync(id.Value);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name");
            ViewData["ValidationErrors"] = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Degree,MinimumDegree,Hours,DepartmentId")] Course course)
        {
            _logger.LogInformation("Create attempt: Name={Name}, DepartmentId={DepartmentId}", course.Name, course.DepartmentId);

            var departments = await _departmentService.GetAllAsync();
            if (!departments.Any(d => d.Id == course.DepartmentId))
            {
                ModelState.AddModelError("DepartmentId", $"Department ID {course.DepartmentId} does not exist.");
                _logger.LogWarning("Invalid DepartmentId: {DepartmentId}", course.DepartmentId);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("ModelState is invalid. Errors: {Errors}", string.Join("; ", errors));
                ViewData["ValidationErrors"] = string.Join("<br>", errors);
                ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name", course.DepartmentId);
                return View(course);
            }

            try
            {
                await _courseService.AddAsync(course);
                if (course.Id == 0)
                {
                    throw new InvalidOperationException("Course ID was not assigned after save.");
                }
                _logger.LogInformation("Course created successfully with ID: {Id}", course.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error: {Message}", ex.InnerException?.Message);
                ModelState.AddModelError("", "Database error: Unable to save. Check Department ID. Details: " + ex.InnerException?.Message);
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

            var modelErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewData["ValidationErrors"] = string.Join("<br>", modelErrors);
            ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name", course.DepartmentId);
            return View(course);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseService.GetByIdAsync(id.Value);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name", course.DepartmentId);
            ViewData["ValidationErrors"] = "";
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Degree,MinimumDegree,Hours,DepartmentId")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            _logger.LogInformation("Edit attempt: ID={Id}, Name={Name}, DepartmentId={DepartmentId}", course.Id, course.Name, course.DepartmentId);

            var departments = await _departmentService.GetAllAsync();
            if (!departments.Any(d => d.Id == course.DepartmentId))
            {
                ModelState.AddModelError("DepartmentId", $"Department ID {course.DepartmentId} does not exist.");
                _logger.LogWarning("Invalid DepartmentId: {DepartmentId}", course.DepartmentId);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("ModelState is invalid. Errors: {Errors}", string.Join("; ", errors));
                ViewData["ValidationErrors"] = string.Join("<br>", errors);
                ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name", course.DepartmentId);
                return View(course);
            }

            try
            {
                await _courseService.UpdateAsync(course);
                _logger.LogInformation("Course updated successfully with ID: {Id}", course.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("DbUpdateConcurrencyException occurred");
                ModelState.AddModelError("", "Unable to save changes. The course was deleted by another user.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error: {Message}", ex.InnerException?.Message);
                ModelState.AddModelError("", "Database error: Unable to save. Check Department ID. Details: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                ModelState.AddModelError("", "An unexpected error occurred. Try again. Details: " + ex.Message);
            }

            var modelErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewData["ValidationErrors"] = string.Join("<br>", modelErrors);
            ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name", course.DepartmentId);
            return View(course);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseService.GetByIdAsync(id.Value);
            if (course == null)
            {
                return NotFound();
            }

            ViewData["ValidationErrors"] = "";
            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _courseService.DeleteAsync(id);
                _logger.LogInformation("Course deleted successfully with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course: {Message}", ex.Message);
                ModelState.AddModelError("", "Unable to delete. Try again. Details: " + ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}