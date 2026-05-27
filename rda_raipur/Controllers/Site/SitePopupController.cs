using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models.site;
using rda_raipur.Filters;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers.Site
{
    [Authorize(Roles = "Admin,Employee")]
    [ServiceFilter(typeof(CheckPermissionAttribute))]
    public class SitePopupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string viewPath = "~/Views/AdminDashboard/Master/SitePopup/";

        public SitePopupController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ==========================================
        // 1. INDEX
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var data = await _context.SitePopups
                                     .Where(x => !x.IsDeleted)
                                     .OrderByDescending(x => x.CreatedDate)
                                     .ToListAsync();
            return View(viewPath + "Index.cshtml", data);
        }

        // ==========================================
        // 2. CREATE
        // ==========================================
        public IActionResult Create()
        {
            return View(viewPath + "Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DisplayOrder, Title, Url, ImageUpload, IsActive")] SitePopup model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImageUpload != null)
                {
                    model.ImagePath = await UploadImageAsync(model.ImageUpload, "popup");
                }

                model.CreatedDate = DateTime.Now;
                model.created_by = User.Identity.Name ?? "Admin";
                model.IsDeleted = false;

                _context.SitePopups.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Popup created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "Create.cshtml", model);
        }

        // ==========================================
        // 3. EDIT
        // ==========================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.SitePopups.FindAsync(id);
            if (item == null || item.IsDeleted) return NotFound();

            return View(viewPath + "Edit.cshtml", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, DisplayOrder, Title, Url, ImageUpload, IsActive")] SitePopup model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.SitePopups.FindAsync(id);
                    if (existing == null) return NotFound();

                    // Update fields
                    existing.DisplayOrder = model.DisplayOrder;
                    existing.Title = model.Title;
                    existing.Url = model.Url;
                    existing.IsActive = model.IsActive;

                    // Handle Image Upload
                    if (model.ImageUpload != null)
                    {
                        DeleteOldFile(existing.ImagePath);
                        existing.ImagePath = await UploadImageAsync(model.ImageUpload, "popup");
                    }

                    // Audit Update
                    existing.updated_by = User.Identity.Name ?? "Admin";
                    existing.updated_Date = DateTime.Now;

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
            return View(viewPath + "Edit.cshtml", model);
        }

        // ==========================================
        // 4. DELETE
        // ==========================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.SitePopups.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true;
                item.IsActive = false;
                item.updated_by = User.Identity.Name ?? "Admin";
                item.updated_Date = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Popup deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper Methods
        private async Task<string> UploadImageAsync(Microsoft.AspNetCore.Http.IFormFile file, string folderName)
        {
            string folderPath = Path.Combine(_env.WebRootPath, "uploads", folderName);
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(folderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return "/uploads/" + folderName + "/" + uniqueFileName;
        }

        private void DeleteOldFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string oldPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            }
        }
    }
}