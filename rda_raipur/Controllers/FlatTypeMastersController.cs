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
    public class FlatTypeMastersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string viewPath = "~/Views/AdminDashboard/Master/FlatTypeMasters/";

        public FlatTypeMastersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============================
        // 1. INDEX
        // ==============================
        public async Task<IActionResult> Index()
        {
            var data = await _context.FlatTypeMasters
                                .Where(x => x.IsDeleted == false) // Safe nullable check
                                .ToListAsync();

            return View(viewPath + "Index.cshtml", data);
        }

        // ==============================
        // 2. DETAILS
        // ==============================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var flatTypeMaster = await _context.FlatTypeMasters
                .FirstOrDefaultAsync(m => m.Flat_id == id);

            if (flatTypeMaster == null) return NotFound();

            return View(viewPath + "Details.cshtml", flatTypeMaster);
        }

        // ==============================
        // 3. CREATE
        // ==============================
        public IActionResult Create()
        {
            return View(viewPath + "Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlatTypeMaster model)
        {
            // 🔥 FIX: Background dynamic properties को validation से हटाया गया
            ModelState.Remove("Flat_id");
            ModelState.Remove("created_by");
            ModelState.Remove("Create_Date");
            ModelState.Remove("updated_by");
            ModelState.Remove("updated_Date");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("IsActive");

            if (ModelState.IsValid)
            {
                model.Create_Date = DateTime.Now;
                model.created_by = User.Identity.Name ?? "Admin";
                model.IsDeleted = false;

                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "Create.cshtml", model);
        }

        // ==============================
        // 4. EDIT
        // ==============================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var flatTypeMaster = await _context.FlatTypeMasters.FindAsync(id);
            if (flatTypeMaster == null || flatTypeMaster.IsDeleted == true) return NotFound();

            return View(viewPath + "Edit.cshtml", flatTypeMaster);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FlatTypeMaster model)
        {
            if (id != model.Flat_id) return NotFound();

            // 🔥 FIX: Validation cleaning
            ModelState.Remove("created_by");
            ModelState.Remove("Create_Date");
            ModelState.Remove("updated_by");
            ModelState.Remove("updated_Date");
            ModelState.Remove("IsDeleted");

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.FlatTypeMasters.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.Flat_name_en = model.Flat_name_en;
                    existing.Flat_name_hi = model.Flat_name_hi;
                    existing.IsActive = model.IsActive;

                    existing.updated_by = User.Identity.Name ?? "Admin";
                    existing.updated_Date = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlatTypeMasterExists(model.Flat_id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "Edit.cshtml", model);
        }

        // ==============================
        // 5. DELETE
        // ==============================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var flatTypeMaster = await _context.FlatTypeMasters
                .FirstOrDefaultAsync(m => m.Flat_id == id);

            if (flatTypeMaster == null || flatTypeMaster.IsDeleted == true) return NotFound();

            return View(viewPath + "Delete.cshtml", flatTypeMaster);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flat = await _context.FlatTypeMasters.FindAsync(id);
            if (flat != null)
            {
                flat.IsDeleted = true;
                flat.IsActive = false;
                flat.updated_by = User.Identity.Name ?? "Admin";
                flat.updated_Date = DateTime.Now;

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FlatTypeMasterExists(int id)
        {
            return _context.FlatTypeMasters.Any(e => e.Flat_id == id);
        }
    }
}