using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using rda_raipur.Data;
using rda_raipur.Models.site; // SiteNews के लिए
using rda_raipur.Filters;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers.Site
{
    [Authorize(Roles = "Admin,Employee")]
    [ServiceFilter(typeof(CheckPermissionAttribute))]
    public class SiteNewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string viewPath = "~/Views/AdminDashboard/Master/SiteNews/";

        public SiteNewsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: SiteNews
        public async Task<IActionResult> Index()
        {
            var data = await _context.SiteNews
                                .Where(x => !x.IsDeleted)
                                .OrderByDescending(x => x.CreatedDate)
                                .ToListAsync();
            return View(viewPath + "Index.cshtml", data);
        }

        // GET: SiteNews/Create
        public IActionResult Create()
        {
            return View(viewPath + "Create.cshtml");
        }

        // POST: SiteNews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NewsText_en, NewsText_hi, LinkUrl, PdfFile, IsActive")] SiteNews model)
        {
            if (ModelState.IsValid)
            {
                // PDF Upload
                if (model.PdfFile != null && model.PdfFile.Length > 0)
                {
                    model.PdfFilePath = await UploadPdfAsync(model.PdfFile);
                }

                // Audit
                model.CreatedDate = DateTime.Now;
                model.created_by = User.Identity.Name ?? "Admin";
                model.IsDeleted = false;

                _context.SiteNews.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "News created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "Create.cshtml", model);
        }

        // GET: SiteNews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var siteNews = await _context.SiteNews.FindAsync(id);
            if (siteNews == null || siteNews.IsDeleted) return NotFound();

            return View(viewPath + "Edit.cshtml", siteNews);
        }

        // POST: SiteNews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, NewsText_en, NewsText_hi, LinkUrl, PdfFile, IsActive")] SiteNews model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.SiteNews.FindAsync(id);
                    if (existing == null) return NotFound();

                    // Update fields
                    existing.NewsText_en = model.NewsText_en;
                    existing.NewsText_hi = model.NewsText_hi;
                    existing.LinkUrl = model.LinkUrl;
                    existing.IsActive = model.IsActive;

                    // Handle PDF
                    if (model.PdfFile != null && model.PdfFile.Length > 0)
                    {
                        DeleteOldFile(existing.PdfFilePath);
                        existing.PdfFilePath = await UploadPdfAsync(model.PdfFile);
                    }

                    // Audit Update
                    existing.updated_by = User.Identity.Name ?? "Admin";
                    existing.updated_Date = DateTime.Now;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "News updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.SiteNews.Any(e => e.Id == model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewPath + "Edit.cshtml", model);
        }

        // POST: SiteNews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.SiteNews.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true;
                item.IsActive = false;
                item.updated_by = User.Identity.Name ?? "Admin";
                item.updated_Date = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "News deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> UploadPdfAsync(Microsoft.AspNetCore.Http.IFormFile file)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "news");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return "/uploads/news/" + uniqueFileName;
        }

        private void DeleteOldFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string oldPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            }
        }
    }
}