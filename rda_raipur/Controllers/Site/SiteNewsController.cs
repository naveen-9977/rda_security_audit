using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using rda_raipur.Data;
using rda_raipur.Models;
using rda_raipur.Models.site;
using rda_raipur.Filters; // 🔥 PERMISSION FILTER KE LIYE
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers.Site
{
    // 🔥 ADMIN & EMPLOYEE DONO KE LIYE + SECURE FILTER
    [Authorize(Roles = "Admin,Employee")]
    [ServiceFilter(typeof(CheckPermissionAttribute))]
    public class SiteNewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SiteNewsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: SiteNews
        public async Task<IActionResult> Index()
        {
            var data = await _context.SiteNews.Where(x => !x.IsDeleted).ToListAsync();
            return View("~/Views/AdminDashboard/Master/SiteNews/Index.cshtml", data);
        }

        // GET: SiteNews/Create
        public IActionResult Create()
        {
            return View("~/Views/AdminDashboard/Master/SiteNews/Create.cshtml");
        }

        // POST: SiteNews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SiteNews model)
        {
            if (ModelState.IsValid)
            {
                // PDF Upload Logic
                if (model.PdfFile != null && model.PdfFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "news");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.PdfFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PdfFile.CopyToAsync(fileStream);
                    }

                    model.PdfFilePath = "/uploads/news/" + uniqueFileName;
                }

                model.CreatedDate = DateTime.Now;
                model.IsActive = true;
                model.IsDeleted = false;
                _context.SiteNews.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "News created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminDashboard/Master/SiteNews/Create.cshtml", model);
        }

        // GET: SiteNews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var siteNews = await _context.SiteNews.FindAsync(id);
            if (siteNews == null || siteNews.IsDeleted) return NotFound();

            return View("~/Views/AdminDashboard/Master/SiteNews/Edit.cshtml", siteNews);
        }

        // POST: SiteNews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SiteNews model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingNews = await _context.SiteNews.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (existingNews == null) return NotFound();

                    // PDF Upload Logic for Edit
                    if (model.PdfFile != null && model.PdfFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "news");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.PdfFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.PdfFile.CopyToAsync(fileStream);
                        }

                        model.PdfFilePath = "/uploads/news/" + uniqueFileName;

                        // Delete Old PDF if exists
                        if (!string.IsNullOrEmpty(existingNews.PdfFilePath))
                        {
                            string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingNews.PdfFilePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                    }
                    else
                    {
                        // Retain existing PDF path if no new file is uploaded
                        model.PdfFilePath = existingNews.PdfFilePath;
                    }

                    model.CreatedDate = existingNews.CreatedDate;
                    model.IsDeleted = false;

                    _context.Update(model);
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
            return View("~/Views/AdminDashboard/Master/SiteNews/Edit.cshtml", model);
        }

        // POST: SiteNews/Delete/5
        // 🔥 Isko HttpPost banaya gaya hai Data security ke liye
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.SiteNews.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true; // Soft delete
                item.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "News deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}