using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models;
using rda_raipur.Filters; // 🔥 PERMISSION FILTER

namespace rda_raipur.Controllers
{
    // 🔥 ADMIN & EMPLOYEE DONO KE LIYE + FILTER
    [Authorize(Roles = "Admin,Employee")]
    [ServiceFilter(typeof(CheckPermissionAttribute))]
    public class Designation_MasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Designation_MasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _context.Designation_Masters.Where(x => x.IsDeleted == false).ToListAsync();
            return View("~/Views/AdminDashboard/Master/Designation_Master/Index.cshtml", data);
        }

        public IActionResult Create()
        {
            return View("~/Views/AdminDashboard/Master/Designation_Master/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Designation_Master designation_Master)
        {
            if (ModelState.IsValid)
            {
                designation_Master.Create_Date = DateTime.Now;
                designation_Master.created_by = User.Identity.Name ?? "Admin";
                designation_Master.IsActive = true;
                designation_Master.IsDeleted = false;

                _context.Add(designation_Master);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Designation added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminDashboard/Master/Designation_Master/Create.cshtml", designation_Master);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var designation_Master = await _context.Designation_Masters.FindAsync(id);
            if (designation_Master == null) return NotFound();
            return View("~/Views/AdminDashboard/Master/Designation_Master/Edit.cshtml", designation_Master);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Designation_Master designation_Master)
        {
            if (id != designation_Master.designation_id) return NotFound();

            if (ModelState.IsValid)
            {
                designation_Master.updated_Date = DateTime.Now;
                designation_Master.updated_by = User.Identity.Name ?? "Admin";

                _context.Update(designation_Master);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Designation updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminDashboard/Master/Designation_Master/Edit.cshtml", designation_Master);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var desig = await _context.Designation_Masters.FindAsync(id);
            if (desig != null)
            {
                desig.IsDeleted = true;
                desig.IsActive = false;
                desig.updated_Date = DateTime.Now;
                desig.updated_by = User.Identity.Name ?? "Admin";

                _context.Update(desig);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Designation deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}