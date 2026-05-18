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
    // 🔴 Yahan se class-level filter hata diya hai, ab hum har action par alag filter lagayenge
    public class BrochureController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BrochureController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ==============================
        // 1. GET: View All Brochures
        // ==============================
        // 🔥 Sirf jiske paas "CanView" hai wo list dekh payega
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanView" })]
        public async Task<IActionResult> Index()
        {
            var brochures = await _context.Brochures.OrderByDescending(b => b.CreatedDate).ToListAsync();
            return View(brochures);
        }

        // ==============================
        // 2. GET: Create Brochure
        // ==============================
        // 🔥 Jiske paas "CanAdd" hai wahi ye page khol payega
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanAdd" })]
        public IActionResult Create()
        {
            return View();
        }

        // ==============================
        // 3. POST: Create Brochure
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanAdd" })]
        public async Task<IActionResult> Create(Brochure model, IFormFile imageFile, IFormFile pdfFile)
        {
            ModelState.Remove("ImagePath");
            ModelState.Remove("PdfPath");

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string imageFolder = Path.Combine(_env.WebRootPath, "images", "brochures");
                        if (!Directory.Exists(imageFolder)) Directory.CreateDirectory(imageFolder);
                        string imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        string imagePath = Path.Combine(imageFolder, imageFileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create)) { await imageFile.CopyToAsync(stream); }
                        model.ImagePath = "/images/brochures/" + imageFileName;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Please upload a thumbnail image.");
                        return View(model);
                    }

                    if (pdfFile != null && pdfFile.Length > 0)
                    {
                        string pdfFolder = Path.Combine(_env.WebRootPath, "documents", "brochures");
                        if (!Directory.Exists(pdfFolder)) Directory.CreateDirectory(pdfFolder);
                        string pdfFileName = Guid.NewGuid().ToString() + Path.GetExtension(pdfFile.FileName);
                        string pdfPath = Path.Combine(pdfFolder, pdfFileName);
                        using (var stream = new FileStream(pdfPath, FileMode.Create)) { await pdfFile.CopyToAsync(stream); }
                        model.PdfPath = "/documents/brochures/" + pdfFileName;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Please upload a PDF brochure.");
                        return View(model);
                    }

                    _context.Brochures.Add(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Brochure added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error saving file: " + ex.Message);
                }
            }
            return View(model);
        }

        // ==============================
        // 4. GET: Edit Brochure
        // ==============================
        // 🔥 Jiske paas "CanEdit" hai wahi edit kar payega
        [HttpGet]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanEdit" })]
        public async Task<IActionResult> Edit(int id)
        {
            var brochure = await _context.Brochures.FindAsync(id);
            if (brochure == null)
            {
                return NotFound();
            }
            return View(brochure);
        }

        // ==============================
        // 5. POST: Edit Brochure
        // ==============================
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
                try
                {
                    var existingBrochure = await _context.Brochures.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (existingBrochure == null) return NotFound();

                    model.ImagePath = existingBrochure.ImagePath;
                    model.PdfPath = existingBrochure.PdfPath;
                    model.CreatedDate = existingBrochure.CreatedDate;

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string imageFolder = Path.Combine(_env.WebRootPath, "images", "brochures");
                        if (!Directory.Exists(imageFolder)) Directory.CreateDirectory(imageFolder);
                        string imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        string imagePath = Path.Combine(imageFolder, imageFileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create)) { await imageFile.CopyToAsync(stream); }
                        model.ImagePath = "/images/brochures/" + imageFileName;
                    }

                    if (pdfFile != null && pdfFile.Length > 0)
                    {
                        string pdfFolder = Path.Combine(_env.WebRootPath, "documents", "brochures");
                        if (!Directory.Exists(pdfFolder)) Directory.CreateDirectory(pdfFolder);
                        string pdfFileName = Guid.NewGuid().ToString() + Path.GetExtension(pdfFile.FileName);
                        string pdfPath = Path.Combine(pdfFolder, pdfFileName);
                        using (var stream = new FileStream(pdfPath, FileMode.Create)) { await pdfFile.CopyToAsync(stream); }
                        model.PdfPath = "/documents/brochures/" + pdfFileName;
                    }

                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Brochure updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating: " + ex.Message);
                }
            }
            return View(model);
        }

        // ==============================
        // 6. POST: Delete/Deactivate
        // ==============================
        // 🔥 Jiske paas "CanDelete" hai wahi delete kar payega
        [HttpPost]
        [TypeFilter(typeof(CheckPermissionAttribute), Arguments = new object[] { "CanDelete" })]
        public async Task<IActionResult> Delete(int id)
        {
            var brochure = await _context.Brochures.FindAsync(id);
            if (brochure != null)
            {
                _context.Brochures.Remove(brochure);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Brochure deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}