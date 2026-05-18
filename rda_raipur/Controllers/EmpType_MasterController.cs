using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Filters; // 🔥 PERMISSION FILTER
using rda_raipur.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers
{
    // 🔥 ADMIN & EMPLOYEE DONO KE LIYE + SECURE FILTER
    [Authorize(Roles = "Admin,Employee")]
    [ServiceFilter(typeof(CheckPermissionAttribute))]
    public class EmpType_MasterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string viewBasePath = "~/Views/AdminDashboard/Master/EmpType_Master/";

        public EmpType_MasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. INDEX
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var empTypes = await _context.EmpType_Masters
                .Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.emp_type_id)
                .ToListAsync();

            return View(viewBasePath + "Index.cshtml", empTypes);
        }

        // ==========================================
        // 2. CREATE
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            return View(viewBasePath + "Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("emp_type_name_hi,emp_type_name_en,IsActive")] EmpType_Master empType_Master)
        {
            if (ModelState.IsValid)
            {
                empType_Master.Create_Date = DateTime.Now;
                empType_Master.created_by = User.Identity.Name ?? "Admin";
                empType_Master.IsDeleted = false;

                _context.Add(empType_Master);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Employee Type Created Successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewBasePath + "Create.cshtml", empType_Master);
        }

        // ==========================================
        // 3. EDIT
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var empType_Master = await _context.EmpType_Masters.FindAsync(id);
            if (empType_Master == null) return NotFound();

            return View(viewBasePath + "Edit.cshtml", empType_Master);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("emp_type_id,emp_type_name_hi,emp_type_name_en,IsActive,Create_Date,created_by")] EmpType_Master empType_Master)
        {
            if (id != empType_Master.emp_type_id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    empType_Master.updated_Date = DateTime.Now;
                    empType_Master.updated_by = User.Identity.Name ?? "Admin";

                    _context.Update(empType_Master);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Employee Type Updated Successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpType_MasterExists(empType_Master.emp_type_id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewBasePath + "Edit.cshtml", empType_Master);
        }

        // ==========================================
        // 4. DELETE (POST ONLY - Same as others)
        // ==========================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empType_Master = await _context.EmpType_Masters.FindAsync(id);
            if (empType_Master != null)
            {
                empType_Master.IsDeleted = true;
                empType_Master.IsActive = false;
                empType_Master.updated_Date = DateTime.Now;
                empType_Master.updated_by = User.Identity.Name ?? "Admin";

                _context.Update(empType_Master);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Employee Type Deleted Successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EmpType_MasterExists(int id)
        {
            return _context.EmpType_Masters.Any(e => e.emp_type_id == id);
        }
    }
}