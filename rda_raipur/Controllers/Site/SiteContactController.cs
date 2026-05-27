using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Filters;
using rda_raipur.Models.site;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers.Site
{
    [Authorize(Roles = "Admin,Employee")]
    public class SiteContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string viewPath = "~/Views/AdminDashboard/Master/SiteContact/";

        public SiteContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============================
        // 1. INDEX
        // ==============================
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanView" })]
        public async Task<IActionResult> Index()
        {
            var data = await _context.SiteContacts
                                     .Where(x => !x.IsDeleted) // Soft delete filter
                                     .OrderByDescending(x => x.CreatedDate)
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
        public async Task<IActionResult> Create(SiteContact model)
        {
            if (ModelState.IsValid)
            {
                // 🔥 Single Active Logic
                if (model.IsActive)
                {
                    var activeContacts = await _context.SiteContacts.Where(c => c.IsActive && !c.IsDeleted).ToListAsync();
                    foreach (var c in activeContacts) { c.IsActive = false; }
                    _context.UpdateRange(activeContacts);
                }

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User.Identity.Name ?? "Admin";
                model.IsDeleted = false;

                _context.SiteContacts.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Contact created successfully!";
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
            var item = await _context.SiteContacts.FindAsync(id);
            if (item == null || item.IsDeleted) return NotFound();
            return View(viewPath + "Edit.cshtml", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanEdit" })]
        public async Task<IActionResult> Edit(int id, SiteContact model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.SiteContacts.FindAsync(id);
                if (existing == null) return NotFound();

                // 🔥 Single Active Logic
                if (model.IsActive)
                {
                    var activeContacts = await _context.SiteContacts.Where(c => c.IsActive && c.Id != id && !c.IsDeleted).ToListAsync();
                    foreach (var c in activeContacts) { c.IsActive = false; }
                    _context.UpdateRange(activeContacts);
                }

                // Update Fields
                existing.OfficeName = model.OfficeName;
                existing.OfficeNameHi = model.OfficeNameHi;
                existing.Address = model.Address;
                existing.AddressHi = model.AddressHi;
                existing.Phone1 = model.Phone1;
                existing.Phone2 = model.Phone2;
                existing.Email = model.Email;
                existing.MapEmbedUrl = model.MapEmbedUrl;
                existing.IsActive = model.IsActive;

                // Audit Fields
                existing.UpdatedBy = User.Identity.Name ?? "Admin";
                existing.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contact updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "Edit.cshtml", model);
        }

        // ==============================
        // 4. DELETE (SOFT DELETE)
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanDelete" })]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.SiteContacts.FindAsync(id);
            if (item != null)
            {
                // Soft Delete
                item.IsDeleted = true;
                item.UpdatedBy = User.Identity.Name ?? "Admin";
                item.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contact deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}