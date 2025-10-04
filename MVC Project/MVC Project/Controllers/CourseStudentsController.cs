using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_Project.Interfaces;
using MVC_Project.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace MVC_Project.Controllers
{
    public class CourseStudentsController : Controller
    {
        private readonly ICourseStudentsService _courseStudentsService;
        private readonly ICourseService _courseService;
        private readonly IStudentService _studentService;
        private readonly ILogger<CourseStudentsController> _logger;

        public CourseStudentsController(ICourseStudentsService courseStudentsService, ICourseService courseService, IStudentService studentService, ILogger<CourseStudentsController> logger)
        {
            _courseStudentsService = courseStudentsService;
            _courseService = courseService;
            _studentService = studentService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var courseStudents = await _courseStudentsService.GetAllAsync();
            return View(courseStudents);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseStudents = await _courseStudentsService.GetByIdAsync(id.Value);
            if (courseStudents == null)
            {
                return NotFound();
            }

            return View(courseStudents);
        }

        public async Task<IActionResult> Create()
        {
            var courses = await _courseService.GetAllAsync();
            var students = await _studentService.GetAllAsync();
            if (!courses.Any())
            {
                ViewData["ValidationErrors"] = "No courses available. Please add a course first.";
                _logger.LogWarning("No courses found when loading Create view.");
                return View();
            }
            if (!students.Any())
            {
                ViewData["ValidationErrors"] = string.IsNullOrEmpty(ViewData["ValidationErrors"]?.ToString())
                    ? "No students available. Please add a student first."
                    : ViewData["ValidationErrors"] + "<br>No students available. Please add a student first.";
                _logger.LogWarning("No students found when loading Create view.");
                return View();
            }
            ViewData["CourseId"] = new SelectList(courses, "Id", "Name");
            ViewData["StudentId"] = new SelectList(students, "Id", "Name");
            ViewData["ValidationErrors"] = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseStudents courseStudents)
        {
            _logger.LogInformation("Create attempt: CourseId={CourseId}, StudentId={StudentId}, Degree={Degree}, FormData={FormData}",
                courseStudents.CourseId, courseStudents.StudentId, courseStudents.Degree,
                string.Join("; ", Request.Form.Select(kvp => $"{kvp.Key}={kvp.Value}")));

            // Log ModelState before validation
            _logger.LogInformation("ModelState entries: {ModelState}",
                string.Join("; ", ModelState.Select(kvp => $"{kvp.Key}: Value={kvp.Value.RawValue ?? "null"}, Errors={string.Join(", ", kvp.Value.Errors.Select(e => e.ErrorMessage))}")));

            var courses = await _courseService.GetAllAsync();
            var students = await _studentService.GetAllAsync();

            // Check for valid CourseId and StudentId
            if (courseStudents.CourseId <= 0 || !courses.Any(c => c.Id == courseStudents.CourseId))
            {
                ModelState.AddModelError("CourseId", "Please select a valid Course.");
                _logger.LogWarning("Invalid CourseId: {CourseId}. Available Course IDs: {CourseIds}",
                    courseStudents.CourseId, string.Join(", ", courses.Select(c => c.Id)));
            }
            if (courseStudents.StudentId <= 0 || !students.Any(s => s.Id == courseStudents.StudentId))
            {
                ModelState.AddModelError("StudentId", "Please select a valid Student.");
                _logger.LogWarning("Invalid StudentId: {StudentId}. Available Student IDs: {StudentIds}",
                    courseStudents.StudentId, string.Join(", ", students.Select(s => s.Id)));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("ModelState is invalid. Errors: {Errors}, FormData={FormData}",
                    string.Join("; ", errors), string.Join("; ", Request.Form.Select(kvp => $"{kvp.Key}={kvp.Value}")));
                ViewData["ValidationErrors"] = string.Join("<br>", errors);
                ViewData["CourseId"] = new SelectList(courses, "Id", "Name", courseStudents.CourseId);
                ViewData["StudentId"] = new SelectList(students, "Id", "Name", courseStudents.StudentId);
                return View(courseStudents);
            }

            try
            {
                await _courseStudentsService.AddAsync(courseStudents);
                if (courseStudents.Id == 0)
                {
                    throw new InvalidOperationException("CourseStudents ID was not assigned after save.");
                }
                _logger.LogInformation("CourseStudents created successfully with ID: {Id}", courseStudents.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error: {Message}", ex.InnerException?.Message);
                ModelState.AddModelError("", "Database error: Unable to save. Check Course or Student ID. Details: " + ex.InnerException?.Message);
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
            ViewData["CourseId"] = new SelectList(courses, "Id", "Name", courseStudents.CourseId);
            ViewData["StudentId"] = new SelectList(students, "Id", "Name", courseStudents.StudentId);
            return View(courseStudents);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseStudents = await _courseStudentsService.GetByIdAsync(id.Value);
            if (courseStudents == null)
            {
                return NotFound();
            }

            var courses = await _courseService.GetAllAsync();
            var students = await _studentService.GetAllAsync();
            if (!courses.Any())
            {
                ViewData["ValidationErrors"] = "No courses available. Please add a course first.";
                _logger.LogWarning("No courses found when loading Edit view.");
                return View(courseStudents);
            }
            if (!students.Any())
            {
                ViewData["ValidationErrors"] = string.IsNullOrEmpty(ViewData["ValidationErrors"]?.ToString())
                    ? "No students available. Please add a student first."
                    : ViewData["ValidationErrors"] + "<br>No students available. Please add a student first.";
                _logger.LogWarning("No students found when loading Edit view.");
                return View(courseStudents);
            }
            ViewData["CourseId"] = new SelectList(courses, "Id", "Name", courseStudents.CourseId);
            ViewData["StudentId"] = new SelectList(students, "Id", "Name", courseStudents.StudentId);
            ViewData["ValidationErrors"] = "";
            return View(courseStudents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseStudents courseStudents)
        {
            if (id != courseStudents.Id)
            {
                return NotFound();
            }

            _logger.LogInformation("Edit attempt: ID={Id}, CourseId={CourseId}, StudentId={StudentId}, Degree={Degree}, FormData={FormData}",
                courseStudents.Id, courseStudents.CourseId, courseStudents.StudentId, courseStudents.Degree,
                string.Join("; ", Request.Form.Select(kvp => $"{kvp.Key}={kvp.Value}")));

            // Log ModelState before validation
            _logger.LogInformation("ModelState entries: {ModelState}",
                string.Join("; ", ModelState.Select(kvp => $"{kvp.Key}: Value={kvp.Value.RawValue ?? "null"}, Errors={string.Join(", ", kvp.Value.Errors.Select(e => e.ErrorMessage))}")));

            var courses = await _courseService.GetAllAsync();
            var students = await _studentService.GetAllAsync();

            // Check for valid CourseId and StudentId
            if (courseStudents.CourseId <= 0 || !courses.Any(c => c.Id == courseStudents.CourseId))
            {
                ModelState.AddModelError("CourseId", "Please select a valid Course.");
                _logger.LogWarning("Invalid CourseId: {CourseId}. Available Course IDs: {CourseIds}",
                    courseStudents.CourseId, string.Join(", ", courses.Select(c => c.Id)));
            }
            if (courseStudents.StudentId <= 0 || !students.Any(s => s.Id == courseStudents.StudentId))
            {
                ModelState.AddModelError("StudentId", "Please select a valid Student.");
                _logger.LogWarning("Invalid StudentId: {StudentId}. Available Student IDs: {StudentIds}",
                    courseStudents.StudentId, string.Join(", ", students.Select(s => s.Id)));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("ModelState is invalid. Errors: {Errors}, FormData={FormData}",
                    string.Join("; ", errors), string.Join("; ", Request.Form.Select(kvp => $"{kvp.Key}={kvp.Value}")));
                ViewData["ValidationErrors"] = string.Join("<br>", errors);
                ViewData["CourseId"] = new SelectList(courses, "Id", "Name", courseStudents.CourseId);
                ViewData["StudentId"] = new SelectList(students, "Id", "Name", courseStudents.StudentId);
                return View(courseStudents);
            }

            try
            {
                await _courseStudentsService.UpdateAsync(courseStudents);
                _logger.LogInformation("CourseStudents updated successfully with ID: {Id}", courseStudents.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("DbUpdateConcurrencyException occurred");
                ModelState.AddModelError("", "Unable to save changes. The courseStudents was deleted by another user.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error: {Message}", ex.InnerException?.Message);
                ModelState.AddModelError("", "Database error: Unable to save. Check Course or Student ID. Details: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                ModelState.AddModelError("", "An unexpected error occurred. Try again. Details: " + ex.Message);
            }

            var modelErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewData["ValidationErrors"] = string.Join("<br>", modelErrors);
            ViewData["CourseId"] = new SelectList(courses, "Id", "Name", courseStudents.CourseId);
            ViewData["StudentId"] = new SelectList(students, "Id", "Name", courseStudents.StudentId);
            return View(courseStudents);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseStudents = await _courseStudentsService.GetByIdAsync(id.Value);
            if (courseStudents == null)
            {
                return NotFound();
            }

            ViewData["ValidationErrors"] = "";
            return View(courseStudents);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _courseStudentsService.DeleteAsync(id);
                _logger.LogInformation("CourseStudents deleted successfully with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting courseStudents: {Message}", ex.Message);
                ModelState.AddModelError("", "Unable to delete. Try again. Details: " + ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}