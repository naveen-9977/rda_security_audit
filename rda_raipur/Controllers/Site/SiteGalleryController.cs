using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models;
using rda_raipur.Models.site;
using rda_raipur.Filters; // 🔥 PERMISSION FILTER ADDED
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace rda_raipur.Controllers.Site
{
    // 🔥 ADMIN & EMPLOYEE DONO KE LIYE + SECURE FILTER
    [Authorize(Roles = "Admin,Employee")]
    [ServiceFilter(typeof(CheckPermissionAttribute))]
    public class SiteGalleryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

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

            return View("~/Views/AdminDashboard/Master/SiteGallery/Index.cshtml", images);
        }

        // ==========================================
        // 1. IMAGE MANAGEMENT ACTIONS
        // ==========================================

        public IActionResult Create()
        {
            ViewBag.Folders = new SelectList(_context.GalleryFolders.Where(x => !x.IsDeleted && x.IsActive), "Id", "FolderName");
            return View("~/Views/AdminDashboard/Master/SiteGallery/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SiteGallery model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImageUpload != null)
                {
                    string folderPath = Path.Combine(_env.WebRootPath, "uploads", "Gallery");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImageUpload.FileName);
                    string filePath = Path.Combine(folderPath, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageUpload.CopyToAsync(fileStream);
                    }
                    model.ImagePath = "/uploads/Gallery/" + uniqueFileName;
                }

                model.CreatedDate = DateTime.Now;
                model.IsActive = true;
                model.IsDeleted = false;

                _context.SiteGalleries.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Image uploaded successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Folders = new SelectList(_context.GalleryFolders.Where(x => !x.IsDeleted && x.IsActive), "Id", "FolderName", model.FolderId);
            return View("~/Views/AdminDashboard/Master/SiteGallery/Create.cshtml", model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.SiteGalleries.FindAsync(id);
            if (item == null || item.IsDeleted) return NotFound();

            ViewBag.Folders = new SelectList(_context.GalleryFolders.Where(x => !x.IsDeleted && x.IsActive), "Id", "FolderName", item.FolderId);
            return View("~/Views/AdminDashboard/Master/SiteGallery/Edit.cshtml", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SiteGallery model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingItem = await _context.SiteGalleries.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (existingItem == null) return NotFound();

                    if (model.ImageUpload != null)
                    {
                        string folderPath = Path.Combine(_env.WebRootPath, "uploads", "Gallery");
                        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImageUpload.FileName);
                        string filePath = Path.Combine(folderPath, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageUpload.CopyToAsync(fileStream);
                        }
                        model.ImagePath = "/uploads/Gallery/" + uniqueFileName;
                    }
                    else
                    {
                        model.ImagePath = existingItem.ImagePath;
                    }

                    model.CreatedDate = existingItem.CreatedDate;
                    model.IsDeleted = false;

                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Image updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.SiteGalleries.Any(e => e.Id == model.Id)) return NotFound();
                    else throw;
                }
            }

            ViewBag.Folders = new SelectList(_context.GalleryFolders.Where(x => !x.IsDeleted && x.IsActive), "Id", "FolderName", model.FolderId);
            return View("~/Views/AdminDashboard/Master/SiteGallery/Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.SiteGalleries.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Image deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // 2. FOLDER / ALBUM MANAGEMENT ACTIONS
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFolder(GalleryFolder model)
        {
            if (model.CoverImageUpload != null)
            {
                string folderPath = Path.Combine(_env.WebRootPath, "uploads", "GalleryCovers");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.CoverImageUpload.FileName);
                string filePath = Path.Combine(folderPath, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CoverImageUpload.CopyToAsync(fileStream);
                }
                model.CoverImagePath = "/uploads/GalleryCovers/" + uniqueFileName;
            }

            model.CreatedDate = DateTime.Now;
            model.IsActive = true;
            model.IsDeleted = false;

            _context.GalleryFolders.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Folder created successfully!";
            TempData["ActiveTab"] = "folders";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFolder(int id)
        {
            var item = await _context.GalleryFolders.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Folder deleted successfully!";
            }
            TempData["ActiveTab"] = "folders";
            return RedirectToAction(nameof(Index));
        }
    }
}