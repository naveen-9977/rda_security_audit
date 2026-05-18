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
    public class AllotmentType_MasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AllotmentType_MasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========================================================
        // 🛡️ PERMISSION HELPER FUNCTION
        // ========================================================
        private async Task<bool> HasPermission(string actionType)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role == "Admin") return true;

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return false;

            var permission = await _context.EmployeePermissions
                .Include(p => p.AppModule)
                .FirstOrDefaultAsync(p => p.UserId == userId && p.AppModule.ControllerName == "AllotmentType_Master");

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

        // GET: AllotmentType_Master
        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> Index()
        {
            // Sirf wahi data layenge jo delete nahi hua hai
            var data = await _context.AllotementTypeMasters
                                     .Where(x => x.IsDeleted == false)
                                     .ToListAsync();
            return View(data);
        }

        // GET: AllotmentType_Master/Details/5
        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var allotmentType_Master = await _context.AllotementTypeMasters
                .FirstOrDefaultAsync(m => m.allotement_type_id == id);

            if (allotmentType_Master == null) return NotFound();

            return View(allotmentType_Master);
        }

        // GET: AllotmentType_Master/Create
        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public IActionResult Create()
        {
            return View();
        }

        // POST: AllotmentType_Master/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public IActionResult Create(AllotmentType_Master model)
        {
            if (ModelState.IsValid)
            {
                model.Create_Date = DateTime.Now;
                model.IsActive = true;
                model.IsDeleted = false;

                _context.AllotementTypeMasters.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: AllotmentType_Master/Edit/5
        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var allotmentType_Master = await _context.AllotementTypeMasters.FindAsync(id);
            if (allotmentType_Master == null) return NotFound();

            return View(allotmentType_Master);
        }

        // POST: AllotmentType_Master/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> Edit(int id, [Bind("allotement_type_id,allotement_type_name_hi,allotement_type_name_en,created_by,Create_Date,updated_Date,updated_by,IsActive,IsDeleted")] AllotmentType_Master allotmentType_Master)
        {
            if (id != allotmentType_Master.allotement_type_id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Audit date update kar rahe hain
                    allotmentType_Master.updated_Date = DateTime.Now;

                    _context.Update(allotmentType_Master);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AllotmentType_MasterExists(allotmentType_Master.allotement_type_id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(allotmentType_Master);
        }

        // GET: AllotmentType_Master/Delete/5
        [HttpGet]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var allotmentType_Master = await _context.AllotementTypeMasters
                .FirstOrDefaultAsync(m => m.allotement_type_id == id);

            if (allotmentType_Master == null) return NotFound();

            return View(allotmentType_Master);
        }

        // POST: AllotmentType_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(CheckPermissionAttribute))] // Security Added
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var allotmentType_Master = await _context.AllotementTypeMasters.FindAsync(id);
            if (allotmentType_Master != null)
            {
                // 🔥 Naya Soft Delete Logic (Record hamesha ke liye delete na ho)
                allotmentType_Master.IsDeleted = true;
                allotmentType_Master.updated_Date = DateTime.Now;
                _context.AllotementTypeMasters.Update(allotmentType_Master);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AllotmentType_MasterExists(int id)
        {
            return _context.AllotementTypeMasters.Any(e => e.allotement_type_id == id);
        }
    }
}