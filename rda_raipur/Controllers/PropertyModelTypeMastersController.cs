using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PropertyModelTypeMastersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string viewPath = "~/Views/AdminDashboard/Master/PropertyModelTypeMasters/";

        public PropertyModelTypeMastersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============================
        // 1. INDEX
        // ==============================
        public async Task<IActionResult> Index()
        {
            var data = await _context.PropertyModelTypeMasters
                               // 🔥 FIX: Nullable bool compatibility check
                               .Where(x => x.IsDeleted == false)
                               .ToListAsync();

            return View(data);
        }

        // ==============================
        // 2. DETAILS
        // ==============================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var propertyModelTypeMaster = await _context.PropertyModelTypeMasters
                .FirstOrDefaultAsync(m => m.Model_Id == id);

            if (propertyModelTypeMaster == null) return NotFound();

            return View(propertyModelTypeMaster);
        }

        // ==============================
        // 3. CREATE
        // ==============================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyModelTypeMaster model)
        {
            // 🔥 FIX: Remove auto-filled system properties from model validation state
            ModelState.Remove("Model_Id");
            ModelState.Remove("created_by");
            ModelState.Remove("Create_Date");
            ModelState.Remove("updated_by");
            ModelState.Remove("updated_Date");
            ModelState.Remove("IsDeleted");

            if (ModelState.IsValid)
            {
                model.Create_Date = DateTime.Now;
                model.created_by = User.Identity.Name ?? "Admin";
                model.IsDeleted = false;

                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // ==============================
        // 4. EDIT
        // ==============================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var propertyModelTypeMaster = await _context.PropertyModelTypeMasters.FindAsync(id);
            if (propertyModelTypeMaster == null || propertyModelTypeMaster.IsDeleted == true) return NotFound();

            return View(propertyModelTypeMaster);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PropertyModelTypeMaster model)
        {
            if (id != model.Model_Id) return NotFound();

            // 🔥 FIX: Edit state input filtering
            ModelState.Remove("created_by");
            ModelState.Remove("Create_Date");
            ModelState.Remove("updated_by");
            ModelState.Remove("updated_Date");

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.PropertyModelTypeMasters.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.Model_name_en = model.Model_name_en;
                    existing.Model_name_hi = model.Model_name_hi;
                    existing.IsActive = model.IsActive;
                    existing.IsDeleted = model.IsDeleted;

                    existing.updated_by = User.Identity.Name ?? "Admin";
                    existing.updated_Date = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyModelTypeMasterExists(model.Model_Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // ==============================
        // 5. DELETE
        // ==============================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var propertyModelTypeMaster = await _context.PropertyModelTypeMasters
                .FirstOrDefaultAsync(m => m.Model_Id == id);

            if (propertyModelTypeMaster == null || propertyModelTypeMaster.IsDeleted == true) return NotFound();

            return View(propertyModelTypeMaster);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var property = await _context.PropertyModelTypeMasters.FindAsync(id);
            if (property != null)
            {
                property.IsDeleted = true;
                property.IsActive = false;
                property.updated_by = User.Identity.Name ?? "Admin";
                property.updated_Date = DateTime.Now;

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyModelTypeMasterExists(int id)
        {
            return _context.PropertyModelTypeMasters.Any(e => e.Model_Id == id);
        }
    }
}