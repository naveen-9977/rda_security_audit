using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models.site; // SitePopup model namespace
using rda_raipur.Filters; // 🔥 PERMISSION FILTER
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers.Site
{
    // 🔥 ADMIN & EMPLOYEE DONO KE LIYE + SECURE FILTER
    [Authorize(Roles = "Admin,Employee")]
    [ServiceFilter(typeof(CheckPermissionAttribute))]
    public class SitePopupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SitePopupController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ==========================================
        // 1. INDEX
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var popups = await _context.SitePopups
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            return View("~/Views/AdminDashboard/Master/SitePopup/Index.cshtml", popups);
        }

        // ==========================================
        // 2. CREATE
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/AdminDashboard/Master/SitePopup/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SitePopup model)
        {
            if (ModelState.IsValid)
            {
                // 🔥 Image Upload Logic
                if (model.ImageUpload != null && model.ImageUpload.Length > 0)
                {
                    string folderPath = Path.Combine(_env.WebRootPath, "uploads", "popup");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImageUpload.FileName);
                    string filePath = Path.Combine(folderPath, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageUpload.CopyToAsync(fileStream);
                    }

                    model.ImagePath = "/uploads/popup/" + uniqueFileName;
                }

                model.CreatedDate = DateTime.Now;
                model.IsActive = true;
                model.IsDeleted = false;

                _context.SitePopups.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Popup added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminDashboard/Master/SitePopup/Create.cshtml", model);
        }

        // ==========================================
        // 3. EDIT
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var popup = await _context.SitePopups.FindAsync(id);
            if (popup == null || popup.IsDeleted) return NotFound();

            return View("~/Views/AdminDashboard/Master/SitePopup/Edit.cshtml", popup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SitePopup model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPopup = await _context.SitePopups.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (existingPopup == null) return NotFound();

                    // 🔥 Image Upload Logic for Edit
                    if (model.ImageUpload != null && model.ImageUpload.Length > 0)
                    {
                        string folderPath = Path.Combine(_env.WebRootPath, "uploads", "popup");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImageUpload.FileName);
                        string filePath = Path.Combine(folderPath, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageUpload.CopyToAsync(fileStream);
                        }

                        model.ImagePath = "/uploads/popup/" + uniqueFileName;

                        // Purani image ko delete karna (agar exist karti hai)
                        if (!string.IsNullOrEmpty(existingPopup.ImagePath))
                        {
                            string oldFilePath = Path.Combine(_env.WebRootPath, existingPopup.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                    }
                    else
                    {
                        // Agar nayi image upload nahi hui, toh purani path ko hi rakhein
                        model.ImagePath = existingPopup.ImagePath;
                    }

                    model.CreatedDate = existingPopup.CreatedDate;
                    model.IsDeleted = false;

                    _context.Update(model);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Popup updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.SitePopups.Any(e => e.Id == model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminDashboard/Master/SitePopup/Edit.cshtml", model);
        }

        // ==========================================
        // 4. DELETE (POST ONLY)
        // ==========================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.SitePopups.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true; // Soft delete
                item.IsActive = false; // Popup ko inactive bhi kar diya
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Popup deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}