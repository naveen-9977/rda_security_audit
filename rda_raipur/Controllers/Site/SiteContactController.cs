using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models.site;
using rda_raipur.Filters; // 🔥 PERMISSION FILTER
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers.Site
{
    // 🔥 ADMIN & EMPLOYEE DONO KE LIYE + SECURE FILTER
    [Authorize(Roles = "Admin,Employee")]
    [ServiceFilter(typeof(CheckPermissionAttribute))]
    public class SiteContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SiteContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. INDEX
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Yahan IsDeleted false filter add kiya hai (agar table me hai) 
            // Agar aapke model me IsDeleted nahi hai, toh .Where() hata de.
            var contacts = await _context.SiteContacts
                // .Where(c => !c.IsDeleted) 
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            return View("~/Views/AdminDashboard/Master/SiteContact/Index.cshtml", contacts);
        }

        // ==========================================
        // 2. CREATE
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/AdminDashboard/Master/SiteContact/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SiteContact siteContact)
        {
            if (ModelState.IsValid)
            {
                // 🔥 Logic: If this is set to active, deactivate all others
                if (siteContact.IsActive)
                {
                    var activeContacts = await _context.SiteContacts.Where(c => c.IsActive).ToListAsync();
                    foreach (var c in activeContacts)
                    {
                        c.IsActive = false;
                    }
                    _context.UpdateRange(activeContacts);
                }

                siteContact.CreatedDate = DateTime.Now;
                // siteContact.IsDeleted = false; // Agar use karte hain toh uncomment karein

                _context.Add(siteContact);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Contact created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminDashboard/Master/SiteContact/Create.cshtml", siteContact);
        }

        // ==========================================
        // 3. EDIT
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var siteContact = await _context.SiteContacts.FindAsync(id);
            if (siteContact == null) return NotFound();

            return View("~/Views/AdminDashboard/Master/SiteContact/Edit.cshtml", siteContact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SiteContact siteContact)
        {
            if (id != siteContact.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingContact = await _context.SiteContacts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (existingContact == null) return NotFound();

                    // 🔥 Logic: If this is set to active, deactivate all others
                    if (siteContact.IsActive)
                    {
                        var activeContacts = await _context.SiteContacts.Where(c => c.IsActive && c.Id != id).ToListAsync();
                        foreach (var c in activeContacts)
                        {
                            c.IsActive = false;
                        }
                        _context.UpdateRange(activeContacts);
                    }

                    siteContact.CreatedDate = existingContact.CreatedDate;
                    // siteContact.IsDeleted = existingContact.IsDeleted; // Agar use karte hain toh uncomment karein

                    _context.Update(siteContact);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Contact updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SiteContactExists(siteContact.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminDashboard/Master/SiteContact/Edit.cshtml", siteContact);
        }

        // ==========================================
        // 4. DELETE (POST ONLY - Safe Delete)
        // ==========================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var siteContact = await _context.SiteContacts.FindAsync(id);
            if (siteContact != null)
            {
                // Hard Delete logic as per your original code
                _context.SiteContacts.Remove(siteContact);

                // Ya agar Soft Delete chahiye toh ye use karein:
                // siteContact.IsDeleted = true;
                // siteContact.IsActive = false;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contact deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SiteContactExists(int id)
        {
            return _context.SiteContacts.Any(e => e.Id == id);
        }
    }
}