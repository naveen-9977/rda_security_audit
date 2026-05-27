using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class PropertyModelMastersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string viewPath = "~/Views/AdminDashboard/Master/PropertyModelMasters/";

        public PropertyModelMastersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============================
        // 1. INDEX
        // ==============================
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanView" })]
        public async Task<IActionResult> Index()
        {
            var data = await _context.PropertyModelMasters
                                     .Include(p => p.Scheme_Master)
                                     .Include(p => p.Sector_Master)
                                     .Include(p => p.Block_Master)
                                     .Where(x => x.IsDeleted == false) // Nullable bool handle safe
                                     .OrderByDescending(x => x.Create_Date)
                                     .ToListAsync();
            return View(viewPath + "Index.cshtml", data);
        }

        // ==============================
        // 2. CREATE
        // ==============================
        [HttpGet]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanAdd" })]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(viewPath + "Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanAdd" })]
        public async Task<IActionResult> Create(PropertyModelMaster model)
        {
            // 🔥 FIX: Background dynamic properties को validation से हटाया गया
            ModelState.Remove("Property_Id"); // Primary key
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
                model.IsActive = true; // Default active state set during creation

                _context.PropertyModelMasters.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Property Model added successfully!";
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(model.scheme_id);
            return View(viewPath + "Create.cshtml", model);
        }

        // ==============================
        // 3. EDIT
        // ==============================
        [HttpGet]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanEdit" })]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.PropertyModelMasters.FindAsync(id);
            if (item == null || item.IsDeleted == true) return NotFound();

            PopulateDropdowns(item.scheme_id);
            return View(viewPath + "Edit.cshtml", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanEdit" })]
        public async Task<IActionResult> Edit(int id, PropertyModelMaster model)
        {
            if (id != model.Property_Id) return NotFound();

            // 🔥 FIX: Edit के समय भी validation से हटाया गया
            ModelState.Remove("created_by");
            ModelState.Remove("Create_Date");
            ModelState.Remove("updated_by");
            ModelState.Remove("updated_Date");
            ModelState.Remove("IsDeleted");

            if (ModelState.IsValid)
            {
                var existing = await _context.PropertyModelMasters.FindAsync(id);
                if (existing == null) return NotFound();

                // Update fields
                existing.scheme_id = model.scheme_id;
                existing.sector_id = model.sector_id;
                existing.block_id = model.block_id;
                existing.Property_Type_Name_en = model.Property_Type_Name_en;
                existing.Property_Type_Name_hi = model.Property_Type_Name_hi;
                existing.IsActive = model.IsActive;

                existing.updated_by = User.Identity.Name ?? "Admin";
                existing.updated_Date = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Property Model updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(model.scheme_id);
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
            var item = await _context.PropertyModelMasters.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true; // Soft Delete
                item.updated_by = User.Identity.Name ?? "Admin";
                item.updated_Date = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Property Model deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ==============================
        // HELPER: Dropdowns & JSON
        // ==============================
        private void PopulateDropdowns(int? selectedSchemeId = null)
        {
            ViewBag.SchemeList = new SelectList(_context.Scheme_Master.Where(x => x.IsDeleted == false), "scheme_id", "scheme_name_en", selectedSchemeId);
        }

        [HttpGet]
        public JsonResult GetSectorsByScheme(int schemeId)
        {
            var sectors = _context.Sector_Master
                .Where(x => x.scheme_id == schemeId && x.IsDeleted == false)
                .Select(x => new { x.sector_id, x.sector_name_en })
                .ToList();
            return Json(sectors);
        }
        // ==============================
        // 5. DETAILS
        // ==============================
        [HttpGet]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanView" })]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.PropertyModelMasters
                .Include(p => p.Scheme_Master)
                .Include(p => p.Sector_Master)
                .Include(p => p.Block_Master)
                .FirstOrDefaultAsync(m => m.Property_Id == id);

            if (item == null || item.IsDeleted == true) return NotFound();

            return View(viewPath + "Details.cshtml", item);
        }
        [HttpGet]
        public JsonResult GetBlocksBySector(int sectorId)
        {
            var blocks = _context.Block_Masters
                .Where(x => x.sector_id == sectorId && x.IsDeleted == false)
                .Select(x => new { x.block_id, x.block_name_en })
                .ToList();
            return Json(blocks);
        }
    }
}