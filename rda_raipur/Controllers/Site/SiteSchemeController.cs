using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Filters;
using rda_raipur.Models.site;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers.Site
{
    [Authorize(Roles = "Admin,Employee")]
    public class SiteSchemeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string viewPath = "~/Views/AdminDashboard/Master/SiteScheme/";

        public SiteSchemeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // ==============================
        // 1. INDEX
        // ==============================
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanView" })]
        public async Task<IActionResult> Index()
        {
            var data = await _context.SiteSchemes
                                     // 🔥 FIX 1: !x.IsDeleted की जगह x.IsDeleted != true किया
                                     .Where(x => x.IsDeleted != true)
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
        public async Task<IActionResult> Create(SiteScheme model, IFormFile? ImageFile)
        {
            // Validation skip for fields set by backend
            ModelState.Remove("ImagePath");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("CreatedBy");

            if (ModelState.IsValid)
            {
                if (ImageFile != null) model.ImagePath = await UploadFileAsync(ImageFile, "uploads/schemes");

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User.Identity.Name ?? "Admin";
                model.IsDeleted = false; // Fresh record

                _context.SiteSchemes.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Scheme created successfully!";
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
            var item = await _context.SiteSchemes.FindAsync(id);

            // 🔥 FIX 2: item.IsDeleted की जगह item.IsDeleted == true किया
            if (item == null || item.IsDeleted == true) return NotFound();

            return View(viewPath + "Edit.cshtml", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanEdit" })]
        public async Task<IActionResult> Edit(int id, SiteScheme model, IFormFile? ImageFile)
        {
            if (id != model.Id) return NotFound();

            ModelState.Remove("ImagePath");

            if (ModelState.IsValid)
            {
                var existing = await _context.SiteSchemes.FindAsync(id);
                if (existing == null) return NotFound();

                // Update Fields
                existing.SchemeName = model.SchemeName;
                existing.Location = model.Location;
                existing.MapLocationUrl = model.MapLocationUrl;
                existing.IsActive = model.IsActive;

                if (ImageFile != null)
                {
                    DeleteOldFile(existing.ImagePath);
                    existing.ImagePath = await UploadFileAsync(ImageFile, "uploads/schemes");
                }

                existing.UpdatedBy = User.Identity.Name ?? "Admin";
                existing.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Scheme updated successfully!";
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
            var item = await _context.SiteSchemes.FindAsync(id);
            if (item != null)
            {
                // Soft Delete implementation
                item.IsDeleted = true;
                item.UpdatedBy = User.Identity.Name ?? "Admin";
                item.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Scheme deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ==============================
        // HELPER METHODS
        // ==============================
        private async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, folder);
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            using (var stream = new FileStream(Path.Combine(folderPath, fileName), FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return "/" + folder + "/" + fileName;
        }

        private void DeleteOldFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, path.TrimStart('/'));
                if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
            }
        }
    }
}