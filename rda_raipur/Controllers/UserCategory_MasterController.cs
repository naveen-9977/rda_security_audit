using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models;
using rda_raipur.Filters;

namespace rda_raipur.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    [ServiceFilter(typeof(CheckPermissionAttribute))]
    public class UserCategory_MasterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string viewPath = "~/Views/AdminDashboard/Master/UserCategory_Master/";

        public UserCategory_MasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============================
        // 1. INDEX
        // ==============================
        public async Task<IActionResult> Index()
        {
            var data = await _context.UserCategoryMasters
                                     .Where(x => x.IsDeleted == false) // Nullable bool handle safe
                                     .OrderByDescending(x => x.Create_Date)
                                     .ToListAsync();

            return View(viewPath + "Index.cshtml", data);
        }

        // ==============================
        // 2. CREATE
        // ==============================
        public IActionResult Create()
        {
            return View(viewPath + "Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCategory_Master model)
        {
            // 🔥 FIX: Validation bypass for server side auto-filled fields
            ModelState.Remove("res_category_id");
            ModelState.Remove("Create_Date");
            ModelState.Remove("created_by");
            ModelState.Remove("updated_Date");
            ModelState.Remove("updated_by");
            ModelState.Remove("IsDeleted");

            if (ModelState.IsValid)
            {
                model.Create_Date = DateTime.Now;
                model.created_by = User.Identity.Name ?? "Admin";
                model.IsDeleted = false;

                _context.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "Create.cshtml", model);
        }

        // ==============================
        // 3. EDIT
        // ==============================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.UserCategoryMasters.FindAsync(id);
            if (item == null || item.IsDeleted == true) return NotFound();

            return View(viewPath + "Edit.cshtml", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserCategory_Master model)
        {
            if (id != model.res_category_id) return NotFound();

            ModelState.Remove("Create_Date");
            ModelState.Remove("created_by");
            ModelState.Remove("updated_Date");
            ModelState.Remove("updated_by");
            ModelState.Remove("IsDeleted");

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.UserCategoryMasters.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.res_category_name_hi = model.res_category_name_hi;
                    existing.res_category_name_en = model.res_category_name_en;
                    existing.IsActive = model.IsActive;

                    existing.updated_by = User.Identity.Name ?? "Admin";
                    existing.updated_Date = DateTime.Now;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Category updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserCategory_MasterExists(model.res_category_id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "Edit.cshtml", model);
        }

        // ==============================
        // 4. DELETE
        // ==============================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.UserCategoryMasters.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true;
                item.IsActive = false;
                item.updated_by = User.Identity.Name ?? "Admin";
                item.updated_Date = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserCategory_MasterExists(int id)
        {
            return _context.UserCategoryMasters.Any(e => e.res_category_id == id);
        }
    }
}