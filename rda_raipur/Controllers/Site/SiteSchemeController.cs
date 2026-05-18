using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using rda_raipur.Data;
using rda_raipur.Models.site;
using rda_raipur.Filters;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims; // 🔥 User ki details nikalne ke liye zaroori

namespace rda_raipur.Controllers.Site
{
    // 🔥 ADMIN & EMPLOYEE DONO KE LIYE + SECURE FILTER
    [Authorize(Roles = "Admin,Employee")]
    [ServiceFilter(typeof(CheckPermissionAttribute))]
    public class SiteSchemeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SiteSchemeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // =====================================
        // 1. INDEX (WITH FIX FOR PERMISSION ERROR)
        // =====================================
        public async Task<IActionResult> Index()
        {
            var data = await _context.SiteSchemes.OrderByDescending(x => x.CreatedDate).ToListAsync();

            // 🔥 PERMISSION LOGIC FOR VIEW BUTTONS 🔥
            if (User.IsInRole("Admin"))
            {
                // Admin ko sab allow hai
                ViewBag.CanAdd = true;
                ViewBag.CanEdit = true;
                ViewBag.CanDelete = true;
            }
            else
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdString, out int userId))
                {
                    // 🔥 NAYA LOGIC: Pehle Module dhoondho (Bina Include use kiye)
                    var targetModule = await _context.AppModules
                                        .FirstOrDefaultAsync(m => m.ModuleName.Contains("SiteScheme"));

                    if (targetModule != null)
                    {
                        // Phir us ModuleId aur UserId se permission nikalo
                        var permission = await _context.EmployeePermissions
                            .FirstOrDefaultAsync(p => p.UserId == userId && p.ModuleId == targetModule.ModuleId);

                        if (permission != null)
                        {
                            ViewBag.CanAdd = permission.CanAdd;
                            ViewBag.CanEdit = permission.CanEdit;
                            ViewBag.CanDelete = permission.CanDelete;
                        }
                        else
                        {
                            ViewBag.CanAdd = false;
                            ViewBag.CanEdit = false;
                            ViewBag.CanDelete = false;
                        }
                    }
                    else
                    {
                        ViewBag.CanAdd = false;
                        ViewBag.CanEdit = false;
                        ViewBag.CanDelete = false;
                    }
                }
            }

            return View("~/Views/AdminDashboard/Master/SiteScheme/Index.cshtml", data);
        }

        // =====================================
        // 2. CREATE SCHEME
        // =====================================
        public IActionResult Create()
        {
            return View("~/Views/AdminDashboard/Master/SiteScheme/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SiteScheme model, IFormFile ImageFile)
        {
            // 🔴 ERROR FIX: इन फील्ड्स का वैलिडेशन इग्नोर करें क्योंकि ये बैकएंड से सेट होंगे
            ModelState.Remove("ImagePath");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ImageFile");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("UpdatedBy");
            ModelState.Remove("UpdatedDate");

            if (ModelState.IsValid)
            {
                // 🔥 Image Upload Logic
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "schemes");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    model.ImagePath = "/uploads/schemes/" + uniqueFileName;
                }

                // 🔥 TRACKING DETAILS ADD KARNA
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User.Identity.Name ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.SiteSchemes.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Scheme created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminDashboard/Master/SiteScheme/Create.cshtml", model);
        }

        // =====================================
        // 3. EDIT SCHEME
        // =====================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var scheme = await _context.SiteSchemes.FindAsync(id);
            if (scheme == null) return NotFound();

            return View("~/Views/AdminDashboard/Master/SiteScheme/Edit.cshtml", scheme);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SiteScheme model, IFormFile ImageFile)
        {
            if (id != model.Id) return NotFound();

            ModelState.Remove("ImagePath");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ImageFile");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("UpdatedBy");
            ModelState.Remove("UpdatedDate");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingScheme = await _context.SiteSchemes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (existingScheme == null) return NotFound();

                    // 🔥 Image Upload Logic for Edit
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "schemes");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        model.ImagePath = "/uploads/schemes/" + uniqueFileName;

                        // Delete Old Image if exists
                        if (!string.IsNullOrEmpty(existingScheme.ImagePath))
                        {
                            string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingScheme.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                    }
                    else
                    {
                        // Retain existing Image path if no new file is uploaded
                        model.ImagePath = existingScheme.ImagePath;
                    }

                    // 🔥 PURANA RECORD PRESERVE KARNA
                    model.CreatedDate = existingScheme.CreatedDate;
                    model.CreatedBy = existingScheme.CreatedBy;

                    // 🔥 NAYA UPDATE TRACKING RECORD DAALNA
                    model.UpdatedDate = DateTime.Now;
                    model.UpdatedBy = User.Identity.Name ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

                    _context.Update(model);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Scheme updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.SiteSchemes.Any(e => e.Id == model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/AdminDashboard/Master/SiteScheme/Edit.cshtml", model);
        }

        // =====================================
        // 4. DELETE SCHEME
        // =====================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.SiteSchemes.FindAsync(id);
            if (item != null)
            {
                // Hard Delete (Image ko bhi server se hatana)
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, item.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.SiteSchemes.Remove(item);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Scheme deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}