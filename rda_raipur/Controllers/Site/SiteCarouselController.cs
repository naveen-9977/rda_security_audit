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
    public class SiteCarouselController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string viewPath = "~/Views/AdminDashboard/Master/SiteCarousel/";

        public SiteCarouselController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: SiteCarousel
        public async Task<IActionResult> Index()
        {
            var data = await _context.SiteCarousels
                                     .Where(x => !x.IsDeleted)
                                     .OrderBy(x => x.DisplayOrder)
                                     .ToListAsync();
            return View(viewPath + "Index.cshtml", data);
        }

        // GET: SiteCarousel/Create
        public IActionResult Create()
        {
            return View(viewPath + "Create.cshtml");
        }

        // POST: SiteCarousel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DisplayOrder, Caption_en, Caption_hi, Url, ImageUpload, IsActive")] SiteCarousel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImageUpload != null)
                {
                    model.ImagePath = await UploadImageAsync(model.ImageUpload, "Carousel");
                }

                model.CreatedDate = DateTime.Now;
                model.created_by = User.Identity.Name ?? "Admin";
                model.IsDeleted = false;

                _context.SiteCarousels.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Carousel banner created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "Create.cshtml", model);
        }

        // GET: SiteCarousel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.SiteCarousels.FindAsync(id);
            if (item == null || item.IsDeleted) return NotFound();

            return View(viewPath + "Edit.cshtml", item);
        }

        // POST: SiteCarousel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, DisplayOrder, Caption_en, Caption_hi, Url, ImageUpload, IsActive")] SiteCarousel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.SiteCarousels.FindAsync(id);
                    if (existing == null) return NotFound();

                    // Update fields
                    existing.DisplayOrder = model.DisplayOrder;
                    existing.Caption_en = model.Caption_en;
                    existing.Caption_hi = model.Caption_hi;
                    existing.Url = model.Url;
                    existing.IsActive = model.IsActive;

                    // Handle Image Upload
                    if (model.ImageUpload != null)
                    {
                        DeleteOldFile(existing.ImagePath);
                        existing.ImagePath = await UploadImageAsync(model.ImageUpload, "Carousel");
                    }

                    // Audit Update
                    existing.updated_by = User.Identity.Name ?? "Admin";
                    existing.updated_Date = DateTime.Now;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Carousel banner updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.SiteCarousels.Any(e => e.Id == model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "Edit.cshtml", model);
        }

        // POST: SiteCarousel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.SiteCarousels.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true;
                item.IsActive = false;
                item.updated_by = User.Identity.Name ?? "Admin";
                item.updated_Date = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Carousel banner deleted successfully!";
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