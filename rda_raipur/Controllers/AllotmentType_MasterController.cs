using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Filters;
using rda_raipur.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class AllotmentType_MasterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string viewPath = "~/Views/AdminDashboard/Master/AllotmentType_Master/";

        public AllotmentType_MasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============================
        // 1. INDEX
        // ==============================
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanView" })]
        public async Task<IActionResult> Index()
        {
            var data = await _context.AllotementTypeMasters
                                     // 🔥 FIX: !x.IsDeleted की जगह x.IsDeleted == false कर दिया है 🔥
                                     .Where(x => x.IsDeleted == false)
                                     .OrderByDescending(x => x.Create_Date)
                                     .ToListAsync();
            return View(viewPath + "Index.cshtml", data);
        }

        // ==============================
        // 2. CREATE
        // ==============================
        [HttpGet]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanAdd" })]
        public IActionResult Create() => View(viewPath + "Create.cshtml");

        [HttpPost]
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanAdd" })]
        public async Task<IActionResult> Create(AllotmentType_Master model)
        {
            // Validation se background fields ko remove karna zaroori hai
            ModelState.Remove("allotement_type_id");
            ModelState.Remove("Create_Date");
            ModelState.Remove("created_by");
            ModelState.Remove("updated_Date");
            ModelState.Remove("updated_by");
            ModelState.Remove("IsDeleted"); // 🔥 FIX: ise bhi remove karna zaroori hai

            if (ModelState.IsValid)
            {
                model.Create_Date = DateTime.Now;
                model.created_by = User.Identity.Name ?? "Admin";
                model.IsDeleted = false;
                // model.IsActive = true; (Ye Form checkbox se aa jayega)

                _context.AllotementTypeMasters.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Allotment Type created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "Create.cshtml", model);
        }

        // ==============================
        // 3. EDIT
        // ==============================
        [HttpGet]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanEdit" })]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.AllotementTypeMasters.FindAsync(id);

            // 🔥 FIX: item.IsDeleted की जगह item.IsDeleted == true कर दिया है 🔥
            if (item == null || item.IsDeleted == true) return NotFound();

            return View(viewPath + "Edit.cshtml", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanEdit" })]
        public async Task<IActionResult> Edit(int id, AllotmentType_Master model)
        {
            if (id != model.allotement_type_id) return NotFound();

            // Edit mein bhi non-form fields ko validation se hatao
            ModelState.Remove("Create_Date");
            ModelState.Remove("created_by");
            ModelState.Remove("updated_Date");
            ModelState.Remove("updated_by");

            if (ModelState.IsValid)
            {
                var existing = await _context.AllotementTypeMasters.FindAsync(id);
                if (existing == null) return NotFound();

                // Update Fields
                existing.allotement_type_name_en = model.allotement_type_name_en;
                existing.allotement_type_name_hi = model.allotement_type_name_hi;
                existing.IsActive = model.IsActive;

                // Audit Fields
                existing.updated_by = User.Identity.Name ?? "Admin";
                existing.updated_Date = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Allotment Type updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "Edit.cshtml", model);
        }

        // ==============================
        // 4. DELETE (SOFT)
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanDelete" })]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.AllotementTypeMasters.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true; // Soft Delete
                item.updated_by = User.Identity.Name ?? "Admin";
                item.updated_Date = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Allotment Type deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}