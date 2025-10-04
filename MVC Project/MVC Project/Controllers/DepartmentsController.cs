using Microsoft.AspNetCore.Mvc;
using MVC_Project.Interfaces;
using MVC_Project.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MVC_Project.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(IDepartmentService departmentService, ILogger<DepartmentsController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllAsync();
            return View(departments);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentService.GetByIdAsync(id.Value);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        public IActionResult Create()
        {
            ViewData["ValidationErrors"] = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,ManagerName")] Department department)
        {
            _logger.LogInformation("Create attempt: Name={Name}", department.Name);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("ModelState is invalid. Errors: {Errors}", string.Join("; ", errors));
                ViewData["ValidationErrors"] = string.Join("<br>", errors);
                return View(department);
            }

            try
            {
                await _departmentService.AddAsync(department);
                if (department.Id == 0)
                {
                    throw new InvalidOperationException("Department ID was not assigned after save.");
                }
                _logger.LogInformation("Department created successfully with ID: {Id}", department.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error: {Message}", ex.InnerException?.Message);
                ModelState.AddModelError("", "Database error: Unable to save. Details: " + ex.InnerException?.Message);
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
            return View(department);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentService.GetByIdAsync(id.Value);
            if (department == null)
            {
                return NotFound();
            }
            ViewData["ValidationErrors"] = "";
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ManagerName")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            _logger.LogInformation("Edit attempt: ID={Id}, Name={Name}", department.Id, department.Name);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("ModelState is invalid. Errors: {Errors}", string.Join("; ", errors));
                ViewData["ValidationErrors"] = string.Join("<br>", errors);
                return View(department);
            }

            try
            {
                await _departmentService.UpdateAsync(department);
                _logger.LogInformation("Department updated successfully with ID: {Id}", department.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("DbUpdateConcurrencyException occurred");
                ModelState.AddModelError("", "Unable to save changes. The department was deleted by another user.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error: {Message}", ex.InnerException?.Message);
                ModelState.AddModelError("", "Database error: Unable to save. Details: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                ModelState.AddModelError("", "An unexpected error occurred. Try again. Details: " + ex.Message);
            }

            var modelErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewData["ValidationErrors"] = string.Join("<br>", modelErrors);
            return View(department);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentService.GetByIdAsync(id.Value);
            if (department == null)
            {
                return NotFound();
            }

            ViewData["ValidationErrors"] = "";
            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _departmentService.DeleteAsync(id);
                _logger.LogInformation("Department deleted successfully with ID: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department: {Message}", ex.Message);
                ModelState.AddModelError("", "Unable to delete. Try again. Details: " + ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}