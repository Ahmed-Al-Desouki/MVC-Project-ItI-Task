using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_Project.Interfaces;
using MVC_Project.Models;
using System.Threading.Tasks;

namespace MVC_Project.Controllers
{
    [AllowAnonymous] // Allow anonymous access to all actions
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _authService.LoginAsync(model))
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }

        // GET: /Auth/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            if (ModelState.IsValid)
            {
                if (await _authService.RegisterAsync(model))
                {
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "Registration failed. Please try again.");
            }
            return View(model);
        }

        // POST: /Auth/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login"); // Redirect to Login after logout
        }
    }
}