using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rda_raipur.Data;
using rda_raipur.Models;
using rda_raipur.Models.site;
using rda_raipur.Filters; // 🔥 PERMISSION FILTER ADD KIYA HAI
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace rda_raipur.Controllers.Site
{
    // 🔥 ADMIN & EMPLOYEE DONO KE LIYE + SECURE FILTER
    [Authorize(Roles = "Admin,Employee")]
    [ServiceFilter(typeof(CheckPermissionAttribute))]
    public class SiteCarouselController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

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
            return View("~/Views/AdminDashboard/Master/SiteCarousel/Index.cshtml", data);
        }

        // GET: SiteCarousel/Create
        public IActionResult Create()
        {
            return View("~/Views/AdminDashboard/Master/SiteCarousel/Create.cshtml");
        }

        // POST: SiteCarousel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SiteCarousel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImageUpload != null)
                {
                    string folderPath = Path.Combine(_env.WebRootPath, "uploads", "Carousel");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImageUpload.FileName);
                    string filePath = Path.Combine(folderPath, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageUpload.CopyToAsync(fileStream);
                    }
                    model.ImagePath = "/uploads/Carousel/" + uniqueFileName;
                }

                model.CreatedDate = DateTime.Now;
                model.IsActive = true;
                model.IsDeleted = false;

                _context.SiteCarousels.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Carousel banner created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminDashboard/Master/SiteCarousel/Create.cshtml", model);
        }

        // GET: SiteCarousel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.SiteCarousels.FindAsync(id);
            if (item == null || item.IsDeleted) return NotFound();

            return View("~/Views/AdminDashboard/Master/SiteCarousel/Edit.cshtml", item);
        }

        // POST: SiteCarousel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SiteCarousel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Use AsNoTracking to get original data without tracking conflicts
                    var existingItem = await _context.SiteCarousels.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (existingItem == null) return NotFound();

                    // If a new image is uploaded, handle the file save
                    if (model.ImageUpload != null)
                    {
                        string folderPath = Path.Combine(_env.WebRootPath, "uploads", "Carousel");
                        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImageUpload.FileName);
                        string filePath = Path.Combine(folderPath, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageUpload.CopyToAsync(fileStream);
                        }
                        model.ImagePath = "/uploads/Carousel/" + uniqueFileName;

                        // Purani image ko delete karne ka logic (optional but recommended)
                        if (!string.IsNullOrEmpty(existingItem.ImagePath))
                        {
                            string oldFilePath = Path.Combine(_env.WebRootPath, existingItem.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                    }
                    else
                    {
                        // Keep original image path if no new file provided
                        model.ImagePath = existingItem.ImagePath;
                    }

                    model.CreatedDate = existingItem.CreatedDate; // Preserve original creation date
                    model.IsDeleted = false;

                    _context.Update(model);
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
            return View("~/Views/AdminDashboard/Master/SiteCarousel/Edit.cshtml", model);
        }

        // POST: SiteCarousel/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.SiteCarousels.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true; // Soft delete
                item.IsActive = false; // Inactive bhi kar diya
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Carousel banner deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}