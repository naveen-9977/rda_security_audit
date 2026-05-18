using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Filters;
using rda_raipur.Models;

namespace rda_raipur.Controllers
{
    // 🔥 PERMISSION SECURITY ADDED: Sirf Admin aur Employee allow honge
    [Authorize(Roles = "Admin,Employee")]
    public class PropertyModelMastersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertyModelMastersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========================================================
        // 🛡️ PERMISSION HELPER FUNCTION
        // ========================================================
        private async Task<bool> HasPermission(string actionType)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role == "Admin") return true; // Admin ko hamesha access hai

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return false;

            // Database se check karega ki is user ko is module ki permission hai ya nahi
            var permission = await _context.EmployeePermissions
                .Include(p => p.AppModule)
                .FirstOrDefaultAsync(p => p.UserId == userId && p.AppModule.ControllerName == "PropertyModelMasters");

            if (permission == null) return false;

            return actionType switch
            {
                "View" => permission.CanView == true,
                "Add" => permission.CanAdd == true,
                "Edit" => permission.CanEdit == true,
                "Delete" => permission.CanDelete == true,
                _ => false
            };
        }

        // GET: PropertyModelMasters
        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> Index()
        {
            var data = await _context.PropertyModelMasters
               .Where(x => x.IsDeleted == false)
               .ToListAsync();

            return View(data);
        }

        // GET: PropertyModelMasters/Details/5
        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var propertyModelMaster = await _context.PropertyModelMasters
                .FirstOrDefaultAsync(m => m.Property_Id == id);

            if (propertyModelMaster == null) return NotFound();

            return View(propertyModelMaster);
        }

        // GET: PropertyModelMasters/Create
        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public IActionResult Create()
        {
            return View();
        }

        // POST: PropertyModelMasters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> Create(PropertyModelMaster model)
        {
            if (ModelState.IsValid)
            {
                model.Create_Date = DateTime.Now;
                model.created_by = User?.Identity?.Name ?? "Admin";
                model.IsActive = true; // By default active
                model.IsDeleted = false;

                _context.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: PropertyModelMasters/Edit/5
        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var propertyModelMaster = await _context.PropertyModelMasters.FindAsync(id);
            if (propertyModelMaster == null) return NotFound();

            return View(propertyModelMaster);
        }

        // POST: PropertyModelMasters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> Edit(int id, [Bind("Property_Id,scheme_id,sector_id,block_id,Property_Type_Name_en,Property_Type_Name_hi,created_by,Create_Date,updated_Date,updated_by,IsActive,IsDeleted")] PropertyModelMaster propertyModelMaster)
        {
            if (id != propertyModelMaster.Property_Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    propertyModelMaster.updated_Date = DateTime.Now;
                    propertyModelMaster.updated_by = User?.Identity?.Name ?? "Admin";

                    _context.Update(propertyModelMaster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyModelMasterExists(propertyModelMaster.Property_Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(propertyModelMaster);
        }

        // GET: PropertyModelMasters/Delete/5
        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var propertyModelMaster = await _context.PropertyModelMasters
                .FirstOrDefaultAsync(m => m.Property_Id == id);

            if (propertyModelMaster == null) return NotFound();

            return View(propertyModelMaster);
        }

        // POST: PropertyModelMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propertyModelMaster = await _context.PropertyModelMasters.FindAsync(id);
            if (propertyModelMaster != null)
            {
                // 🔥 Soft Delete: Data database se hard delete na ho
                propertyModelMaster.IsDeleted = true;
                propertyModelMaster.updated_Date = DateTime.Now;
                propertyModelMaster.updated_by = User?.Identity?.Name ?? "Admin";

                _context.PropertyModelMasters.Update(propertyModelMaster);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PropertyModelMasterExists(int id)
        {
            return _context.PropertyModelMasters.Any(e => e.Property_Id == id);
        }
    }
}