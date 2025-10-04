using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_Project.Interfaces;
using MVC_Project.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MVC_Project.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentService studentService, IDepartmentService departmentService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _departmentService = departmentService;
            _logger = logger;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllAsync();
            return View(students);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentService.GetByIdAsync(id.Value);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public async Task<IActionResult> Create()
        {
            var departments = await _departmentService.GetAllAsync();
            _logger.LogInformation("Departments count: {Count}", departments.Count());
            if (!departments.Any())
            {
                ModelState.AddModelError("", "No departments available. Please add departments first.");
            }
            ViewData["DepartmentId"] = new SelectList(departments, "Id", "Name");
            ViewData["ValidationErrors"] = "";
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Address,Grade,DepartmentId")] Student student)
        {
            _logger.LogInformation("Create attempt: Name={Name}, DepartmentId={DepartmentId}", student.Name, student.DepartmentId);

            // Check if ID exists in the database
            var departments = await _departmentService.GetAllAsync();
            if (!departments.Any(d => d.Id == student.DepartmentId))
            {
                ModelState.AddModelError("DepartmentId", $"Department ID {student.DepartmentId} does not exist.");
                _logger.LogWarning("Invalid DepartmentId: {DepartmentId}", student.DepartmentId);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("ModelState is invalid. Errors: {Errors}", string.Join("; ", errors));
                ViewData["ValidationErrors"] = string.Join("<br>", errors);
                ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name", student.DepartmentId);
                return View(student);
            }

            try
            {
                await _studentService.AddAsync(student);
                if (student.Id == 0)
                {
                    throw new InvalidOperationException("Student ID was not assigned after save.");
                }
                _logger.LogInformation("Student created successfully with ID: {Id}", student.Id);
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
            ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name", student.DepartmentId);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentService.GetByIdAsync(id.Value);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name", student.DepartmentId);
            ViewData["ValidationErrors"] = "";
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Grade,DepartmentId")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            _logger.LogInformation("Edit attempt: ID={Id}, Name={Name}, DepartmentId={DepartmentId}", student.Id, student.Name, student.DepartmentId);

            // Check if ID exists in the database
            var departments = await _departmentService.GetAllAsync();
            if (!departments.Any(d => d.Id == student.DepartmentId))
            {
                ModelState.AddModelError("DepartmentId", $"Department ID {student.DepartmentId} does not exist.");
                _logger.LogWarning("Invalid DepartmentId: {DepartmentId}", student.DepartmentId);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("ModelState is invalid. Errors: {Errors}", string.Join("; ", errors));
                ViewData["ValidationErrors"] = string.Join("<br>", errors);
                ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name", student.DepartmentId);
                return View(student);
            }

            try
            {
                await _studentService.UpdateAsync(student);
                _logger.LogInformation("Student updated successfully with ID: {Id}", student.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("DbUpdateConcurrencyException occurred");
                ModelState.AddModelError("", "Unable to save changes. The student was deleted by another user.");
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
            ViewData["DepartmentId"] = new SelectList(await _departmentService.GetAllAsync(), "Id", "Name", student.DepartmentId);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentService.GetByIdAsync(id.Value);
            if (student == null)
            {
                return NotFound();
            }

            ViewData["ValidationErrors"] = "";
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _studentService.DeleteAsync(id);
                _logger.LogInformation("Student deleted successfully with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student: {Message}", ex.Message);
                ModelState.AddModelError("", "Unable to delete. Try again. Details: " + ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}