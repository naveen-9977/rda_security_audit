using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models.PermissionModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AppModuleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppModuleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. LIST VIEW
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // 🔥 Yahan syntax fix kiya gaya hai 🔥
            var modules = await _context.AppModules
                                        .OrderBy(m => m.Category)
                                        .ThenBy(m => m.ModuleName)
                                        .ToListAsync();
            return View(modules);
        }

        // 2. CREATE FORM (GET)
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = GetExistingCategories();
            return View();
        }

        // 3. CREATE ACTION (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppModule model)
        {
            if (ModelState.IsValid)
            {
                model.IsActive = true;
                _context.AppModules.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Module '{model.ModuleName}' added successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = GetExistingCategories();
            return View(model);
        }

        // 🔥 4. EDIT FORM (GET) 🔥
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var module = await _context.AppModules.FindAsync(id);
            if (module == null) return NotFound();

            // Fetch all unique categories for the dropdown
            ViewBag.Categories = await _context.AppModules
                                             .Where(m => !string.IsNullOrEmpty(m.Category))
                                             .Select(m => m.Category)
                                             .Distinct()
                                             .ToListAsync();

            return View(module);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppModule model)
        {
            if (id != model.ModuleId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Module settings updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, reload categories
            ViewBag.Categories = await _context.AppModules
                                             .Where(m => !string.IsNullOrEmpty(m.Category))
                                             .Select(m => m.Category)
                                             .Distinct()
                                             .ToListAsync();
            return View(model);
        }

        // Helper Method for Dropdown
        private List<string> GetExistingCategories()
        {
            return _context.AppModules
                           .Where(m => !string.IsNullOrEmpty(m.Category))
                           .Select(m => m.Category)
                           .Distinct()
                           .ToList();
        }
    }
}