using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class SiteGalleryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string viewPath = "~/Views/AdminDashboard/Master/SiteGallery/";

        public SiteGalleryController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: SiteGallery
        public async Task<IActionResult> Index()
        {
            var images = await _context.SiteGalleries
                                       .Include(g => g.GalleryFolder)
                                       .Where(x => !x.IsDeleted)
                                       .OrderByDescending(x => x.CreatedDate)
                                       .ToListAsync();

            ViewBag.FoldersList = await _context.GalleryFolders
                                                .Where(x => !x.IsDeleted)
                                                .OrderByDescending(x => x.CreatedDate)
                                                .ToListAsync();

            return View(viewPath + "Index.cshtml", images);
        }

        // ==========================================
        // 1. IMAGE MANAGEMENT
        // ==========================================

        public IActionResult Create()
        {
            ViewBag.Folders = new SelectList(_context.GalleryFolders.Where(x => !x.IsDeleted && x.IsActive), "Id", "FolderName");
            return View(viewPath + "Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FolderId, Title_en, Title_hi, ImageUpload, IsActive")] SiteGallery model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImageUpload != null)
                {
                    model.ImagePath = await UploadImageAsync(model.ImageUpload, "Gallery");
                }

                model.CreatedDate = DateTime.Now;
                model.created_by = User.Identity.Name ?? "Admin";
                model.IsDeleted = false;

                _context.SiteGalleries.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Image uploaded successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Folders = new SelectList(_context.GalleryFolders.Where(x => !x.IsDeleted && x.IsActive), "Id", "FolderName", model.FolderId);
            return View(viewPath + "Create.cshtml", model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.SiteGalleries.FindAsync(id);
            if (item == null || item.IsDeleted) return NotFound();

            ViewBag.Folders = new SelectList(_context.GalleryFolders.Where(x => !x.IsDeleted && x.IsActive), "Id", "FolderName", item.FolderId);
            return View(viewPath + "Edit.cshtml", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, FolderId, Title_en, Title_hi, ImageUpload, IsActive")] SiteGallery model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.SiteGalleries.FindAsync(id);
                if (existing == null) return NotFound();

                existing.FolderId = model.FolderId;
                existing.Title_en = model.Title_en;
                existing.Title_hi = model.Title_hi;
                existing.IsActive = model.IsActive;

                if (model.ImageUpload != null)
                {
                    DeleteOldFile(existing.ImagePath);
                    existing.ImagePath = await UploadImageAsync(model.ImageUpload, "Gallery");
                }

                existing.updated_by = User.Identity.Name ?? "Admin";
                existing.updated_Date = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Image updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Folders = new SelectList(_context.GalleryFolders.Where(x => !x.IsDeleted && x.IsActive), "Id", "FolderName", model.FolderId);
            return View(viewPath + "Edit.cshtml", model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.SiteGalleries.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true;
                item.updated_by = User.Identity.Name ?? "Admin";
                item.updated_Date = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Image deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // 2. FOLDER MANAGEMENT
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFolder([Bind("FolderName, CoverImageUpload, IsActive")] GalleryFolder model)
        {
            if (model.CoverImageUpload != null)
            {
                model.CoverImagePath = await UploadImageAsync(model.CoverImageUpload, "GalleryCovers");
            }

            model.CreatedDate = DateTime.Now;
            model.created_by = User.Identity.Name ?? "Admin";
            model.IsDeleted = false;

            _context.GalleryFolders.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Folder created successfully!";
            TempData["ActiveTab"] = "folders";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditFolder(int? id)
        {
            if (id == null) return NotFound();
            var folder = await _context.GalleryFolders.FindAsync(id);
            if (folder == null || folder.IsDeleted) return NotFound();

            return View(viewPath + "EditFolder.cshtml", folder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFolder(int id, [Bind("Id, FolderName, CoverImageUpload, IsActive")] GalleryFolder model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.GalleryFolders.FindAsync(id);
                if (existing == null) return NotFound();

                existing.FolderName = model.FolderName;
                existing.IsActive = model.IsActive;

                if (model.CoverImageUpload != null)
                {
                    DeleteOldFile(existing.CoverImagePath);
                    existing.CoverImagePath = await UploadImageAsync(model.CoverImageUpload, "GalleryCovers");
                }

                existing.updated_by = User.Identity.Name ?? "Admin";
                existing.updated_Date = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Album updated successfully!";
                TempData["ActiveTab"] = "folders";
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "EditFolder.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFolder(int id)
        {
            var item = await _context.GalleryFolders.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true;
                item.updated_by = User.Identity.Name ?? "Admin";
                item.updated_Date = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Folder deleted successfully!";
            }
            TempData["ActiveTab"] = "folders";
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