using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models;
using rda_raipur.Filters; // 🔥 PERMISSION FILTER KE LIYE ADD KIYA HAI

namespace rda_raipur.Controllers
{
    // 🔥 YAHAN ADMIN KE SATH EMPLOYEE ADD KIYA HAI AUR FILTER LAGAYA HAI
    [Authorize(Roles = "Admin,Employee")]
    [ServiceFilter(typeof(CheckPermissionAttribute))]
    public class Department_MasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Department_MasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Department_Master
        public async Task<IActionResult> Index()
        {
            var data = await _context.Department_Masters
                        .Where(x => x.IsDeleted == false)
                        .ToListAsync();

            return View("~/Views/AdminDashboard/Master/Department_Master/Index.cshtml", data);
        }

        // GET: Department_Master/Create
        public IActionResult Create()
        {
            return View("~/Views/AdminDashboard/Master/Department_Master/Create.cshtml");
        }

        // POST: Department_Master/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department_Master department_Master)
        {
            if (ModelState.IsValid)
            {
                department_Master.Create_Date = DateTime.Now;
                department_Master.created_by = User.Identity.Name ?? "Admin";
                department_Master.IsActive = true;
                department_Master.IsDeleted = false;

                _context.Add(department_Master);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Department added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminDashboard/Master/Department_Master/Create.cshtml", department_Master);
        }

        // GET: Department_Master/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var department_Master = await _context.Department_Masters.FindAsync(id);
            if (department_Master == null) return NotFound();

            return View("~/Views/AdminDashboard/Master/Department_Master/Edit.cshtml", department_Master);
        }

        // POST: Department_Master/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department_Master department_Master)
        {
            if (id != department_Master.dpt_id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    department_Master.updated_Date = DateTime.Now;
                    department_Master.updated_by = User.Identity.Name ?? "Admin";

                    _context.Update(department_Master);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Department updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Department_MasterExists(department_Master.dpt_id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminDashboard/Master/Department_Master/Edit.cshtml", department_Master);
        }

        // POST: Department_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dept = await _context.Department_Masters.FindAsync(id);
            if (dept != null)
            {
                dept.IsDeleted = true;
                dept.IsActive = false;
                dept.updated_Date = DateTime.Now;
                dept.updated_by = User.Identity.Name ?? "Admin";

                _context.Update(dept);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Department deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool Department_MasterExists(int id)
        {
            return _context.Department_Masters.Any(e => e.dpt_id == id);
        }
    }
}