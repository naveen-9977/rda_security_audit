using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Filters;
using rda_raipur.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class BrochureController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string viewPath = "~/Views/AdminDashboard/Brochure/";

        public BrochureController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ==============================
        // 1. INDEX
        // ==============================
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanView" })]
        public async Task<IActionResult> Index()
        {
            var brochures = await _context.Brochures
                                          .Where(b => !b.IsDeleted)
                                          .OrderByDescending(b => b.CreatedDate)
                                          .ToListAsync();
            return View(viewPath + "Index.cshtml", brochures);
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
        public async Task<IActionResult> Create(Brochure model, IFormFile? imageFile, IFormFile? pdfFile)
        {
            ModelState.Remove("ImagePath");
            ModelState.Remove("PdfPath");

            if (ModelState.IsValid)
            {
                if (imageFile != null) model.ImagePath = await UploadFileAsync(imageFile, "images/brochures");
                if (pdfFile != null) model.PdfPath = await UploadFileAsync(pdfFile, "documents/brochures");

                model.CreatedDate = DateTime.Now;
                model.created_by = User.Identity.Name ?? "Admin";
                model.IsDeleted = false;

                _context.Brochures.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Brochure added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "Create.cshtml", model);
        }

        // ==============================
        // 3. EDIT
        // ==============================
        [HttpGet]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanEdit" })]
        public async Task<IActionResult> Edit(int id)
        {
            var brochure = await _context.Brochures.FindAsync(id);
            if (brochure == null || brochure.IsDeleted) return NotFound();
            return View(viewPath + "Edit.cshtml", brochure);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanEdit" })]
        public async Task<IActionResult> Edit(int id, Brochure model, IFormFile? imageFile, IFormFile? pdfFile)
        {
            if (id != model.Id) return NotFound();

            ModelState.Remove("ImagePath");
            ModelState.Remove("PdfPath");

            if (ModelState.IsValid)
            {
                var existing = await _context.Brochures.FindAsync(id);
                if (existing == null) return NotFound();

                // Update Fields
                existing.Title_En = model.Title_En;
                existing.Title_Hi = model.Title_Hi;
                existing.Description = model.Description;
                existing.IsActive = model.IsActive;

                // Handle File Updates
                if (imageFile != null)
                {
                    DeleteOldFile(existing.ImagePath);
                    existing.ImagePath = await UploadFileAsync(imageFile, "images/brochures");
                }

                if (pdfFile != null)
                {
                    DeleteOldFile(existing.PdfPath);
                    existing.PdfPath = await UploadFileAsync(pdfFile, "documents/brochures");
                }

                // Audit Fields
                existing.updated_by = User.Identity.Name ?? "Admin";
                existing.updated_Date = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Brochure updated successfully!";
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
            var brochure = await _context.Brochures.FindAsync(id);
            if (brochure != null)
            {
                brochure.IsDeleted = true; // Soft Delete
                brochure.updated_by = User.Identity.Name ?? "Admin";
                brochure.updated_Date = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Brochure deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ==============================
        // HELPER METHODS
        // ==============================
        private async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            string folderPath = Path.Combine(_env.WebRootPath, folder);
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
                string fullPath = Path.Combine(_env.WebRootPath, path.TrimStart('/'));
                if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
            }
        }
    }
}